using System.Blog.Application.Responses;

namespace System.Blog.Application.Interfaces.Users.UserManagement;

public interface IGetAllUsersUseCase
{
    Task<OperationResult<IEnumerable<UserResponse>>> ExecuteAsync();
}
