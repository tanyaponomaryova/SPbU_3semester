using System.Collections.Concurrent;
using System.Reflection;

namespace MyNUnit
{
    /// <summary>
    /// Class methods with corresponding attributes.
    /// </summary>
    public class ClassMethods
    {
        public ConcurrentBag<MethodInfo> MethodsWithTestAttr { get; private set; } = new();
        public ConcurrentBag<MethodInfo> MethodsWithBeforeAttr { get; private set; } = new();
        public ConcurrentBag<MethodInfo> MethodsWithAfterAttr { get; private set; } = new();
        public ConcurrentBag<MethodInfo> MethodsWithBeforeClassAttr { get; private set; } = new();
        public ConcurrentBag<MethodInfo> MethodsWithAfterClassAttr { get; private set; } = new();

        public ClassMethods(Type _class)
        {
            var allMethodsInfo = _class.GetMethods();
            Parallel.ForEach(allMethodsInfo,
            (MethodInfo methodInfo) =>
            {
                if (methodInfo.GetCustomAttribute<TestAttribute>() != null)
                {
                    MethodsWithTestAttr.Add(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute<BeforeAttribute>() != null)
                {
                    MethodsWithBeforeAttr.Add(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute<AfterAttribute>() != null)
                {
                    MethodsWithAfterAttr.Add(methodInfo);
                }
                else if (methodInfo.GetCustomAttribute<BeforeClassAttribute>() != null)
                {
                    if (methodInfo.IsStatic)
                    {
                        MethodsWithBeforeClassAttr.Add(methodInfo);
                    }
                    else
                    {
                        throw new InvalidAttributeUsageException("BeforeClassAttribute must be applied to static methods only.");
                    }
                }
                else if (methodInfo.GetCustomAttribute<AfterClassAttribute>() != null)
                {
                    if (methodInfo.IsStatic)
                    {
                        MethodsWithAfterClassAttr.Add(methodInfo);
                    }
                    else
                    {
                        throw new InvalidAttributeUsageException("AfterClassAttribute must be applied to static methods only.");
                    }
                }
            });
        }
    }
}