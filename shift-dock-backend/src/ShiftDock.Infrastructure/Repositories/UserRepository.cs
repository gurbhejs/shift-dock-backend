using Microsoft.EntityFrameworkCore;
using ShiftDock.Application.Interfaces.Repositories;
using ShiftDock.Domain.Entities;
using ShiftDock.Domain.Enums;
using ShiftDock.Infrastructure.Data;

namespace ShiftDock.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Memberships)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Memberships.Where(m => m.Status == UserStatus.Active))
            .FirstOrDefaultAsync(u => u.Phone == phone, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByOrganizationIdAsync(string organizationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.OrganizationMemberships
            .Where(m => m.OrganizationId == organizationId)
            .Include(m => m.User)
            .Select(m => m.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken = default)
    {
        return await _context.OrganizationMemberships
            .Where(m => m.OrganizationId == organizationId)
            .CountAsync(cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        return user;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
    }
}
