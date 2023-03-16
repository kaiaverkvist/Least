using AutoMapper;
using Least.Operations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Least.Resource;

/// <summary>
/// The Model Resource implements a basic REST API tied to a specific DbContext Model.
/// Use this if you just want an automatic API.
/// Make sure you also tag the resource you create, with a [Resource] attribute.
/// </summary>
/// <example>
/// A Minimal setup involves something like this:
/// <code>
///     public class UserChangeRequest
///     {
///         public string Name { get; set; }
///     }
///     
///     [Resource("users")]
///     public class UserResource : ModelResource<User, UserModel, UserChangeRequest, UserChangeRequest>
///     {
///         public UserResource()
///         {
///             ReadByIdOperation.SetIncludes("Books");
///             ReadAllOperation.SetIncludes("Books");
///         }
///     }
/// </code>
/// </example>
/// <typeparam name="TEntity">The DbContext Entity this Resource will operate on.</typeparam>
/// <typeparam name="TDomainType">AutoMapper will map to this type when fetching the entity.</typeparam>
/// <typeparam name="TWriteType">Type to map from when performing a PUT operation.</typeparam>
/// <typeparam name="TCreateType">Type to map from when creating a POST operation.</typeparam>
public class ModelResource<TEntity, TDomainType, TWriteType, TCreateType> : IResource
    where TEntity : class
    where TWriteType : class
    where TCreateType : class
{
    public ReadByIdOperation<TEntity> ReadByIdOperation = new();
    public ReadAllOperation<TEntity> ReadAllOperation = new();
    public UpdateByIdOperation<TEntity> UpdateByIdOperation = new();
    public DeleteByIdOperation<TEntity> DeleteByIdOperation = new();
    public CreateOperation<TEntity, TCreateType> CreateOperation = new();
    
    /// <summary>
    /// No need to call this yourself in most cases.
    /// Just mark your class with the [Resource] attribute to have enable reflection auto-discovery.
    /// Use the `Bootstrapper` class to initialize your resources. 
    /// </summary>
    /// <param name="name">Name from the Resource attribute's name.</param>
    /// <param name="app">WebApplication instance from ASP Core.</param>
    /// <param name="context">DbContext from the DI Services scope.</param>
    public void Register(string name, WebApplication app)
    {
        // Map Get
        app.MapGet(name + "/", async (
            HttpContext ctx,
            DbContext db,
            IMapper mapper
        ) =>
        {
            if (!ReadAllOperation.CanReadAll(ctx))
                return Results.Unauthorized();

            var results = ReadAllOperation.GetAll(db).ToList();
            return Results.Ok(mapper.Map<List<TDomainType>>(results));
        });
        
        app.MapGet(name + "/{id}", async (
            HttpContext ctx,
            DbContext db,
            IMapper mapper,
            uint id) =>
        {
            var result = await ReadByIdOperation.GetByIdAsync(db, id);
            if (result == null)
                return Results.NotFound();
            
            if (!ReadByIdOperation.CanReadById(ctx, result))
                return Results.Unauthorized();
            
            return Results.Ok(mapper.Map<TDomainType>(result));
        });
        
        app.MapPut(name + "/{id}", async (
            HttpContext ctx,
            DbContext db,
            IMapper mapper,
            uint id,
            [FromBody] TWriteType input
        ) =>
        {
            var entityAtId = await UpdateByIdOperation.GetByIdAsync(db, id);
            if (entityAtId == null)
                return Results.NotFound();

            if (!UpdateByIdOperation.CanUpdateById(ctx, entityAtId))
                return Results.Unauthorized();

            // Overlay the properties from our TWriteType.
            mapper.Map<TWriteType, TEntity>(input, entityAtId);
            
            await UpdateByIdOperation.UpdateById(db, entityAtId);
            return Results.Ok();
        });
        
                
        app.MapDelete(name + "/{id}", async (
            HttpContext ctx,
            DbContext db,
            uint id
        ) =>
        {
            var entityAtId = await DeleteByIdOperation.GetByIdAsync(db, id);
            if (entityAtId == null)
                return Results.NotFound();

            if (!DeleteByIdOperation.CanDeleteById(ctx, entityAtId))
                return Results.Unauthorized();

            await DeleteByIdOperation.DeleteById(db, entityAtId);
            return Results.Ok();
        });

        app.MapPost(name + "/", async (
            HttpContext ctx,
            DbContext db,
            IMapper mapper,
            [FromBody] TCreateType input
        ) =>
        {
            if (!CreateOperation.CanCreate(ctx, input))
                return Results.Unauthorized();

            await CreateOperation.Create(db, mapper.Map<TEntity>(input));
            return Results.Ok();
        });
    }
}