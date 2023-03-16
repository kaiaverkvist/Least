namespace Least.ExampleAPI.Domain;

public class UserModel
{
    public uint Id { get; set; }
    public string Name { get; set; }
    public List<BookModel> Books { get; set; }
}