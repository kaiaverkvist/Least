using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Least.Operations;

public class ReadAllOperation<TEntity>
    where TEntity : class
{
    public Func<HttpContext, bool> CanReadAll = (context) => true;
    private List<string> _includes = new();

    internal IQueryable<TEntity> GetAll(DbContext db)
    {
        
        var query = db.Set<TEntity>().AsQueryable();
        
        _includes.ForEach(i => query = query.Include(i));
        
        return query.AsNoTracking();
    }
    
    /// <summary>
    /// Set a Func type which returns a bool depending on whether the given http context
    /// can get the entity in return.
    /// </summary>
    /// <param name="permission">Permission delegate.</param>
    public void SetPermission(Func<HttpContext, bool> permission) =>
        CanReadAll = permission;

    public void SetIncludes(params string[] includes)
    {
        _includes = includes.ToList();
    }
}