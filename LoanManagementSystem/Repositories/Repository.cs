using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Repositories;

public class Repository<TEntity>(LoanManagementContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly LoanManagementContext Context = context;
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public IQueryable<TEntity> Query() => DbSet.AsQueryable();

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        DbSet.Update(entity);
    }

    public void Remove(TEntity entity) => DbSet.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => Context.SaveChangesAsync(cancellationToken);
}

