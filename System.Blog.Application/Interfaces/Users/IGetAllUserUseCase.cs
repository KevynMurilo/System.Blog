using System.Blog.Application.Responses;

namespace System.Blog.Application.Interfaces.Users;

public interface IGetAllUserUseCase
{
    Task<OperationResult<IEnumerable<UserResponse>>> ExecuteAsync();
}
