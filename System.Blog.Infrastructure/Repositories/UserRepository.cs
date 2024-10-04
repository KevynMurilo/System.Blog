using Microsoft.EntityFrameworkCore;
using System.Blog.Core.Contracts.Repositories;
using System.Blog.Core.Entities;
using System.Blog.Core.Responses;
using System.Blog.Infrastructure.Data;

namespace System.Blog.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        return await _context.Users
                .AsNoTracking()
                .Select(user => new UserResponse
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    PhotoPath = user.PhotoPath
                })
                .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
