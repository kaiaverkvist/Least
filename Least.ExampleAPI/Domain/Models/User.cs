namespace Least.ExampleAPI.Domain.Models;

public class User : BaseModel
{
    public string Name { get; set; }
    
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}