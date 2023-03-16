using Least.ExampleAPI.Domain;
using Least.ExampleAPI.Domain.Models;
using Least.Resource;

namespace Least.ExampleAPI.Resources;

public class BookChangeRequest
{
    public string Name { get; set; }
}

[Resource("books")]
public class BookResource : ModelResource<Book, BookModel, BookChangeRequest, BookChangeRequest>
{
}