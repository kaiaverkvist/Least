﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Least.Operations;

public class ReadAllOperation<TEntity>
    where TEntity : class
{
    public Func<HttpContext, bool> CanReadAll = (context) => true;
    private List<string> _includes = new();
    private Func<DbContext, IQueryable<TEntity>>? _overrideGetAllFunction;


    internal IQueryable<TEntity> GetAll(DbContext db)
    {
        if (_overrideGetAllFunction != null)
            return _overrideGetAllFunction(db);
        
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
    
    /// <summary>
    /// Used if the default function does not suffice. This allows consumers
    /// to define their own queries.
    /// </summary>
    /// <param name="overrideFunc">Func for the override query</param>
    public void SetOverride(Func<DbContext, IQueryable<TEntity>> overrideFunc)
    {
        _overrideGetAllFunction = overrideFunc;
    }
}