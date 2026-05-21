namespace App.Persistence.Repositories;

public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<TEntity> Set;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
        Set     = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
        => await Set.FirstOrDefaultAsync(e => e.Id!.Equals(id), ct);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
        => await Set.AsNoTracking().ToListAsync(ct);

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => await Set.AsNoTracking().Where(predicate).ToListAsync(ct);

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => await Set.AsNoTracking().AnyAsync(predicate, ct);

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null
            ? await Set.AsNoTracking().CountAsync(ct)
            : await Set.AsNoTracking().CountAsync(predicate, ct);

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
        => await Set.AddAsync(entity, ct);

    public virtual void Update(TEntity entity)
        => Set.Update(entity);

    public virtual void Remove(TEntity entity)
        => Set.Remove(entity);

    public virtual void HardRemove(TEntity entity)
        => Context.Entry(entity).State = EntityState.Deleted;
}
