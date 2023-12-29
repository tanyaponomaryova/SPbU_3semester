using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace MyNUnit;

/// <summary>
/// Class for running tests.
/// </summary>
public class MyNUnit
{
    private ConcurrentDictionary<Type, ClassMethods> testMethods = new();

    /// <summary>
    /// Test results for the corresponding class.
    /// </summary>
    public ConcurrentDictionary<Type, ConcurrentBag<TestResults>> TestResults { get; private set; } = new();

    /// <summary>
    /// Returns classes from assemblies at the given path.
    /// </summary>
    private IEnumerable<Type> GetClasses(string path)
    {
        var assemblies = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories).ToList();
        assemblies.RemoveAll(pathToAssembly => pathToAssembly.Contains("\\MyNUnit.dll"));

        var allAssemblies = new List<string>();
        var uniqueAssemblies = new List<string>();
        foreach (var assembly in assemblies)
        {
            if (!allAssemblies.Contains(Path.GetFileName(assembly)))
            {
                allAssemblies.Add(Path.GetFileName(assembly));
                uniqueAssemblies.Add(assembly);
            }
        }

        return uniqueAssemblies.Select(Assembly.LoadFrom)
                                .SelectMany(a => a.ExportedTypes)
                                .Where(t => t.IsClass);

    }

    /// <summary>
    /// Runs tests in assemblies at the given path.
    /// </summary>
    public void RunTests(string path)
    {
        var classes = GetClasses(path);

        Parallel.ForEach(classes, _class =>
        {
            testMethods.TryAdd(_class, new ClassMethods(_class));
        });

        Parallel.ForEach(testMethods.Keys, _class =>
        {
            TestResults.TryAdd(_class, new ConcurrentBag<TestResults>());
            foreach (var beforeClassMethod in testMethods[_class].MethodsWithBeforeClassAttr)
            {
                beforeClassMethod.Invoke(null, null);
            }

            // сделать чтобы тоже параллельно
            foreach (var testMethod in testMethods[_class].MethodsWithTestAttr)
            {
                ExecuteTestMethod(_class, testMethod);
            }

            foreach (var afterClassMethod in testMethods[_class].MethodsWithAfterClassAttr)
            {
                afterClassMethod.Invoke(null, null);
            }
        });
    }

    private void ExecuteTestMethod(Type _class, MethodInfo methodInfo)
    {
        var attribute = methodInfo.GetCustomAttribute<TestAttribute>();
        if (attribute!.ShouldBeIgnored())
        {
            TestResults[_class].Add(new TestResults(methodInfo.Name, attribute!.IgnoreMessage));
            return;
        }

        var isSuccessful = false;
        Type? thrownException = null;
        var сonstructor = _class.GetConstructor(Type.EmptyTypes);
        if (сonstructor == null)
        {
            throw new FormatException($"{_class.Name} must have constructor w/o parameters.");
        }
        // сделать тест резалт для случая если бросается исключение

        var classInstance = сonstructor.Invoke(null);

        Parallel.ForEach(testMethods[_class].MethodsWithBeforeAttr,
            beforeTestMethod => beforeTestMethod.Invoke(classInstance, null));

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            methodInfo.Invoke(classInstance, null);

            if (attribute.ExpectedException == null)
            {
                isSuccessful = true;
            }
        }
        catch (Exception e)
        {
            thrownException = e.InnerException?.GetType();

            if (attribute.ExpectedException == thrownException)
            {
                isSuccessful = true;
            }
        }

        stopwatch.Stop();
        var executionTime = stopwatch.Elapsed;

        // убрать ! 
        TestResults[_class].Add(new TestResults(methodInfo.Name, isSuccessful, attribute!.ExpectedException, thrownException, executionTime));

        Parallel.ForEach(testMethods[_class].MethodsWithAfterAttr,
            afterTestMethod => afterTestMethod.Invoke(classInstance, null));
    }

    /// <summary>
    /// Prints testing results to console.
    /// </summary>
    public void PrintResults()
    {
        Console.WriteLine("Testing results:\n");
        foreach (var _class in TestResults.Keys)
        {
            Console.WriteLine($"Class: {_class}\n");
            foreach (var testInfo in TestResults[_class])
            {
                Console.WriteLine($"Method: {testInfo.MethodName}()");
                if (testInfo.IsIgnored)
                {
                    Console.WriteLine($"Test ignored with message: {testInfo.IgnoreReason}");
                }
                else
                {
                    Console.WriteLine($"Execution time: {testInfo.ExecutionTime} ms");
                    if (testInfo.ExpectedException != null && testInfo.ActualException != null)
                    {
                        Console.WriteLine($"Expected exception: {testInfo.ExpectedException}\nThrown exception: {testInfo.ActualException}");
                    }
                    if (testInfo.ExpectedException == null && testInfo.ActualException != null)
                    {
                        Console.WriteLine($"Unexpected exception: {testInfo.ActualException}");
                    }
                    if (testInfo.IsSuccessful)
                    {
                        Console.WriteLine($"Test has passed");
                    }
                    else
                    {
                        Console.WriteLine($"Test has failed");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}