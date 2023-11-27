using MyNUnit;
using System.Collections.Concurrent;

namespace MyNUnitTests
{
    public class Tests
    {
        string path = "../../../../ProjectForTests";
        MyNUnit.MyNUnit myNUnit = new();
        ConcurrentDictionary<Type, ConcurrentBag<TestResults>> testResults;
        string[] expectedTestMethodsNames = new string[]
        { "SuccessfulTestMethod",
          "TestMethodWithExpectedException",
          "TestMethodWithUnexpectedException",
          "IgnoredTestMethod"
        };

        [SetUp]
        public void Setup()
        {
            myNUnit.RunTests(path);
            testResults = myNUnit.TestResults;
        }

        [NUnit.Framework.Test]
        public void Test1()
        {

        }
    }
}