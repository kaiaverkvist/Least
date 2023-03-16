namespace Least.Resource;

[AttributeUsage(AttributeTargets.Class)]
public class ResourceAttribute : Attribute
{
    public string Name { get; set; }
    
    public ResourceAttribute(string name)
    {
        Name = name;
    }

}