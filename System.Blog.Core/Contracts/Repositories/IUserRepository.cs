using System.Blog.Core.Entities;
using System.Blog.Core.Responses;

namespace System.Blog.Core.Contracts.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> GetByEmailAsync(string email);
    Task AddAsync(User user);
}
