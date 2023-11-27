using MyNUnit;

public class ClassForTests
{
    public void NotTestMethod()
    {

    }

    [Test]
    public void SuccessfulTestMethod()
    {

    }

    [Test(typeof(ArgumentException))]
    public void TestMethodWithExpectedException()
    {
        throw new ArgumentException();
    }

    [Test(typeof(ArgumentException))]
    public void TestMethodWithUnexpectedException()
    {
        throw new ArgumentNullException();
    }

    [Test(ignoreMessage: "This test should be ignored.")]
    public void IgnoredTestMethod()
    {

    }
}