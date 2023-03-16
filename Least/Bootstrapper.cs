using System.Reflection;
using Least.Resource;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Least;

public class Bootstrapper<TContext> where TContext : DbContext
{
    private List<IResource> _resources = new();
    private WebApplication _app;

    public Bootstrapper(WebApplication app)
    {
        _app = app;
        
        RegisterResources();
    }

    private void RegisterResources()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // Get all types in our assembly.
            foreach (Type type in assembly.GetTypes())
            {
                // Get all RouteAttributes.
                var attributes = (ResourceAttribute[])type.GetCustomAttributes(typeof(ResourceAttribute), false);

                // If we find a ScriptAttribute for the given type object.
                if (attributes.Length > 0)
                {
                    // For every one of the ScriptAttributes, add it to the script list.
                    foreach (var resourceAttribute in attributes)
                    {
                        string name = resourceAttribute.Name;
                        if (name == String.Empty)
                            throw new InvalidOperationException(
                                "Resource must have a name defined with the Resource attribute");

                        // Create an instance of the type, with the game server object passed in as a constructor argument.
                        var t = (IResource)Activator.CreateInstance(type)!;
                        t?.Register(name, _app);
                    }
                }
            }
        }
    }
}