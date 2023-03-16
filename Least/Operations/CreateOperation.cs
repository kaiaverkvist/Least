using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Least.Operations;

public class CreateOperation<TEntity, TCreateType>
    where TEntity : class
    where TCreateType : class
{

    public Func<HttpContext, TCreateType, bool> CanCreate = (context, arg2) => true;

    internal async Task Create(DbContext db, TEntity entity)
    {
        db.Set<TEntity>().Add(entity);
        await db.SaveChangesAsync();
    }
    
    /// <summary>
    /// Set a Func type which returns a bool depending on whether the given http context
    /// can get the entity in return.
    /// </summary>
    /// <param name="permission">Permission delegate.</param>
    public void SetPermission( Func<HttpContext, TCreateType, bool> permission) =>
        CanCreate = permission;
}