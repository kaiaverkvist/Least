using Least.ExampleAPI.Domain;
using Least.ExampleAPI.Domain.Models;
using Least.Resource;

namespace Least.ExampleAPI.Resources;

public class UserChangeRequest
{
    public string Name { get; set; }
}

[Resource("users")]
public class UserResource : ModelResource<User, UserModel, UserChangeRequest, UserChangeRequest>
{
    public UserResource()
    {
        ReadByIdOperation.SetIncludes("Books");
        ReadAllOperation.SetIncludes("Books");
    }
}