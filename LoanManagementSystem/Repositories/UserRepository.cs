using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Repositories;

public interface IUserRepository
{
    IQueryable<ApplicationUser> Query();
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<ApplicationUser>> GetByRoleAsync(UserRole role, ApprovalStatus? status, CancellationToken cancellationToken = default);
    Task<List<ApplicationUser>> GetOfficersAsync(ApprovalStatus? status, CancellationToken cancellationToken = default);
    void Update(ApplicationUser user);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class UserRepository(LoanManagementContext context) : IUserRepository
{
    private readonly LoanManagementContext _context = context;

    public IQueryable<ApplicationUser> Query() => _context.Users.AsQueryable();

    public Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<List<ApplicationUser>> GetByRoleAsync(UserRole role, ApprovalStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.Role == role);

        if (status.HasValue)
        {
            query = query.Where(u => u.ApprovalStatus == status.Value);
        }

        return await query.OrderByDescending(u => u.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<List<ApplicationUser>> GetOfficersAsync(ApprovalStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.Role == UserRole.LoanOfficer || u.Role == UserRole.FieldOfficer);

        if (status.HasValue)
        {
            query = query.Where(u => u.ApprovalStatus == status.Value);
        }

        return await query.OrderByDescending(u => u.CreatedAt).ToListAsync(cancellationToken);
    }

    public void Update(ApplicationUser user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}

