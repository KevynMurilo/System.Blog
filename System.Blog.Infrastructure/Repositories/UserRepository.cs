using Microsoft.EntityFrameworkCore;
using System.Blog.Core.Contracts.Repositories;
using System.Blog.Core.Entities;
using System.Blog.Infrastructure.Data;

namespace System.Blog.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
                .AsNoTracking()
                .ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(e => e.Email == email);       
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetUnconfirmedUsersAsync(DateTime expirationDate)
    {
        return await _context.Users
            .Where(u => !u.IsActived && u.CreatedDate < expirationDate)
            .ToListAsync();
    }

    public async Task DeleteAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
