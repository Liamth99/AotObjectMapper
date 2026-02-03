namespace AotObjectMapper.Sample.Models;
#pragma warning disable LOCAT001

public class Parent
{
    public Child Child { get; set; } = null!;
}

public class ParentDto
{
    public ChildDto Child { get; set; } = null!;
}