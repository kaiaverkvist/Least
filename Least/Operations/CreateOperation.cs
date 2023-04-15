using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Least.Operations;

public class CreateOperation<TEntity, TCreateType>
    where TEntity : class
    where TCreateType : class
{

    internal Func<DbContext, HttpContext, TCreateType, bool> CanCreate = (db, context, createEntity) => true;

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
    public void SetPermission(Func<DbContext, HttpContext, TCreateType, bool> permission) =>
        CanCreate = permission;
}