using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;


using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     Base repository for all repositories
/// </summary>
/// <remarks>
///     This class implements the basic CRUD operations for all repositories.
///     It requires the entity type to be passed as a generic parameter.
///     It also requires the context to be passed in the constructor.
/// </remarks>
public class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> where TEntity : class
{
   protected readonly AppDbContext Context;

   protected BaseRepository(AppDbContext context)
   {
      Context = context;
   }

   /// <inheritdoc />
   public async Task AddAsync(TEntity entity)
   {
      await Context.Set<TEntity>().AddAsync(entity);
   }

   public virtual async Task<TEntity?> FindByIdAsync(TId id)
   {
      return await Context.Set<TEntity>().FindAsync(id);
   }
   
   /// <inheritdoc />
   public void Update(TEntity entity)
   {
      Context.Set<TEntity>().Update(entity);
   }

   /// <inheritdoc />
   public void Remove(TEntity entity)
   {
      Context.Set<TEntity>().Remove(entity);
   }

   /// <inheritdoc />
   public async Task<IEnumerable<TEntity>> ListAsync()
   {
      return await Context.Set<TEntity>().ToListAsync();
   }
}