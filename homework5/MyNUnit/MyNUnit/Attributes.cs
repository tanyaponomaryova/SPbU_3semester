namespace MyNUnit;

[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    public Type? ExpectedException { get; set; }

    public string? IgnoreMessage { get; set; }

    public bool ShouldBeIgnored() 
        => IgnoreMessage != null;

    public TestAttribute(Type? expectedException = null, string? ignoreMessage = null)
    {
        ExpectedException = expectedException;
        IgnoreMessage = ignoreMessage;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class BeforeAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class BeforeClassAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class AfterClassAttribute : Attribute { }