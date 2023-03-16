# Least
> An antidote to complex API architectures.

## Motivation

## Is *Least* for me?
Consider the following limitations first:
* ⚠️ The primary key of your entities must be **uint** for ID resolution to work as expected.
* ⚠️ The library is still very much a work in progress.

## How do I use it?
(Documentation is work in progress)

### Resources
You can define a resource like this:

```csharp
public class UserChangeRequest
{
    public string Name { get; set; }
}

[Resource("users")]
public class UserResource : ModelResource<User, UserModel, UserChangeRequest, UserChangeRequest>
{
    public UserResource()
    {
        // The same as doing query.Include("Books")
        ReadByIdOperation.SetIncludes("Books");
        ReadAllOperation.SetIncludes("Books");
    }
}
```
*This sets up a basic resource, taking in four generic parameters. They are as follows:*
* *User*: DbContext entity that the API operates on.
* *UserModel*: DTO/Model entity that User will map to when fetching entities.
* *UserChangeRequest*: The API maps PUT request bodies to this type.
* *UserChangeRequest(2)*: The same as the above, but for creation (POST). Used same type for brevity of example.

### ASP Core setup
* As the internal API type has no knowledge about your specific DbContext, please add this to your builder configuration: `builder.Services.AddScoped<DbContext, ExampleDbContext>();`
* Add `new Bootstrapper<ExampleDbContext>(app);` after calling the `builder.Build()` method.

*Still unsure?* Look at the ExampleAPI project included in this repository.
