namespace Least.ExampleAPI.Domain.Models;

public class Book : BaseModel
{
    public string Name { get; set; }
    
    public uint UserId { get; set; }
    public virtual User User { get; set; }
}