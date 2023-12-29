using MyNUnit;
using System.Collections.Concurrent;

namespace MyNUnitTests
{
    public class Tests
    {
        private string path = "../../../../ProjectForTests";
        private MyNUnit.MyNUnit myNUnit;
        private ConcurrentDictionary<Type, ConcurrentBag<TestResults>> testResults;
        private List<string> expectedTestMethodsNames = new List<string>
        {   "SuccessfulTestMethod",
            "TestMethodWithExpectedException",
            "TestMethodWithUnexpectedException",
            "IgnoredTestMethod"
        };

        [SetUp]
        public void Setup()
        {
            myNUnit = new();
            myNUnit.RunTests(path);
            testResults = myNUnit.TestResults;
        }

        [NUnit.Framework.Test]
        public void OnlyTestsWithTestAttributesAreExecuted()
        {
            var actualTestMethodsNames = new List<string>();

            foreach (var _class in testResults.Keys)
            {
                foreach (var testResult in testResults[_class])
                {
                    actualTestMethodsNames.Add(testResult.MethodName);
                }
            }

            var areListsEquivalent = (actualTestMethodsNames.Count() == expectedTestMethodsNames.Count())
                                     && !actualTestMethodsNames.Except(expectedTestMethodsNames).Any();
            Assert.True(areListsEquivalent);
        }

        [NUnit.Framework.Test]
        public void SuccessfulMethodTestPassed()
        {
            foreach (var testResultBag in testResults.Values)
            {
                foreach (var testResult in testResultBag)
                {
                    if ("SuccessfulTestMethod".Equals(testResult.MethodName))
                    {
                        Assert.True(testResult.IsSuccessful);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public void IgnoredMethodsAreIgnored()
        {
            foreach (var testResultBag in testResults.Values)
            {
                foreach (var testResult in testResultBag)
                {
                    if ("IgnoredTestMethod".Equals(testResult.MethodName))
                    {
                        Assert.True(testResult.IsIgnored && testResult.IgnoreReason!.Equals("This test should be ignored."));
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public void TestOfMethodWithExpectedExceptionPasses()
        {
            foreach (var testResultBag in testResults.Values)
            {
                foreach (var testResult in testResultBag)
                {
                    if ("TestMethodWithExpectedException".Equals(testResult.MethodName))
                    {
                        Assert.That(testResult.ActualException, Is.EqualTo(testResult.ExpectedException));
                        Assert.True(testResult.IsSuccessful);
                    }
                }
            }
        }

        [NUnit.Framework.Test]
        public void TestOfMethodWithUnexpectedExceptionFails()
        {
            foreach (var testResultBag in testResults.Values)
            {
                foreach (var testResult in testResultBag)
                {
                    if ("TestMethodWithUnexpectedException".Equals(testResult.MethodName))
                    {
                        Assert.That(testResult.ActualException, Is.Not.EqualTo(testResult.ExpectedException));
                        Assert.True(!testResult.IsSuccessful);
                    }
                }
            }
        }
    }
}