using System.Blog.Core.Contracts.Repositories;
using System.Blog.Application.Interfaces.Users;
using System.Blog.Core.Responses;

namespace System.Blog.Application.UseCases.Users;

public class GetAllUserUseCase : IGetAllUserUseCase
{
    private readonly IUserRepository _userRepository;

    public GetAllUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OperationResult<IEnumerable<UserResponse>>> ExecuteAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            if (!users.Any()) return new OperationResult<IEnumerable<UserResponse>> { Message = "Users not found", StatusCode = 404 };

            return new OperationResult<IEnumerable<UserResponse>> { Result = users };
        }
        catch (Exception ex)
        {
            return new OperationResult<IEnumerable<UserResponse>> { ReqSuccess = false, Message = $"Unexpected error: {ex.Message}", StatusCode = 500 };
        }
    }
}
