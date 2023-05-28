using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Least.Operations;

public class CreateOperation<TEntity, TCreateType>
    where TEntity : class
    where TCreateType : class
{

    internal Func<HttpContext, TCreateType, bool> CanCreate = (_, _) => true;
    internal Func<HttpContext, TCreateType, TEntity>? TransformerFunc;


    internal async Task Create(HttpContext ctx, TEntity entity)
    {
        var db = ctx.RequestServices.GetRequiredService<DbContext>();

        db.Set<TEntity>().Add(entity);
        await db.SaveChangesAsync();
    }
    
    /// <summary>
    /// Set a Func type which returns a bool depending on whether the given http context
    /// can get the entity in return.
    /// </summary>
    /// <param name="permission">Permission delegate.</param>
    public void SetPermission(Func<HttpContext, TCreateType, bool> permission) =>
        CanCreate = permission;
    
    public void SetTransformer(Func<HttpContext, TCreateType, TEntity> transformerFunc)
    {
        TransformerFunc = transformerFunc;
    }
}