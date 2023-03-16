﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Least.Operations;

public class UpdateByIdOperation<TEntity>
    where TEntity : class
{
    
    public Func<HttpContext, TEntity, bool> CanUpdateById = (context, arg2) => true;
    private Func<DbContext, uint, Task<TEntity?>>? _overrideGetByIdFunc;

    private List<string> _includes = new();

    internal async Task UpdateById(DbContext db, TEntity entity)
    {
        db.Entry(entity).State = EntityState.Modified;
        await db.SaveChangesAsync();
    }

    internal async Task<TEntity?> GetByIdAsync(DbContext db, uint id)
    {
        if (_overrideGetByIdFunc != null)
            return await _overrideGetByIdFunc(db, id);
        
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
        CanUpdateById = permission;

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
    public void SetOverride(Func<DbContext, uint, Task<TEntity?>> overrideFunc)
    {
        _overrideGetByIdFunc = overrideFunc;
    }
}