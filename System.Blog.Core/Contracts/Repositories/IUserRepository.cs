using System.Blog.Core.Entities;

namespace System.Blog.Core.Contracts.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<IEnumerable<User>> GetUnconfirmedUsersAsync(DateTime expirationDate);
    Task DeleteAsync(Guid userId);
}
