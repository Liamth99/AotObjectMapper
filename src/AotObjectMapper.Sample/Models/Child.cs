namespace AotObjectMapper.Sample.Models;
#pragma warning disable LOCAT001

public class Child
{
    public Parent Parent { get; set; } = null!;
}

public class ChildDto
{
    public ParentDto Parent { get; set; } = null!;
}