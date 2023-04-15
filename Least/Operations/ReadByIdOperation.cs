using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Least.Operations;

public class ReadByIdOperation<TEntity>
    where TEntity : class
{
    private List<string> _includes = new();
    
    internal Func<HttpContext, TEntity, bool> CanReadById = (_, _) => true;
    private Func<HttpContext, uint, Task<TEntity?>>? _overrideGetByIdFunc;

    internal async Task<TEntity?> GetByIdAsync(HttpContext ctx, uint id)
    {
        var db = ctx.RequestServices.GetRequiredService<DbContext>();
        
        // If we have an override set, use that instead.
        if (_overrideGetByIdFunc != null)
            return await _overrideGetByIdFunc(ctx, id);
        
        // Finds the primary key property, so we can look for the entity.
        var keyProperty = db.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties[0];
        
        var query = db
            .Set<TEntity>()
            .AsQueryable();
        
        // Includes if we require them for the query.
        _includes.ForEach(i => query = query.Include(i));
        
        return await query
            .FirstOrDefaultAsync(e => EF.Property<uint>(e, keyProperty.Name) == id);
    }

    /// <summary>
    /// Set a Func type which returns a bool depending on whether the given http context
    /// can get the entity in return.
    /// </summary>
    /// <param name="permission">Permission delegate.</param>
    public void SetPermission(Func<HttpContext, TEntity, bool> permission) =>
        CanReadById = permission;

    /// <summary>
    /// SetIncludes should be a list of navigation properties related to the entity.
    /// This is the same as doing .Include("Name")
    /// </summary>
    /// <param name="includes">List of navigation properties by name.</param>
    public void SetIncludes(params string[] includes)
    {
        _includes = includes.ToList();
    }

    /// <summary>
    /// Used if the default function does not suffice. This allows consumers
    /// to define their own queries.
    /// </summary>
    /// <param name="overrideFunc">Func for the override query</param>
    public void SetOverride(Func<HttpContext, uint, Task<TEntity?>> overrideFunc)
    {
        _overrideGetByIdFunc = overrideFunc;
    }
}