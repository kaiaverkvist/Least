using Microsoft.AspNetCore.Builder;

namespace Least.Resource;

public interface IResource
{
    void Register(string name, WebApplication app);
}