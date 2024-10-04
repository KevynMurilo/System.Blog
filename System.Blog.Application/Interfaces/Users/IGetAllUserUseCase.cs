using System.Blog.Core.Responses;

namespace System.Blog.Application.Interfaces.Users;

public interface IGetAllUserUseCase
{
    Task<OperationResult<IEnumerable<UserResponse>>> ExecuteAsync();
}
