using System.Blog.Application.DTOs;
using System.Blog.Application.Responses;

namespace System.Blog.Application.Interfaces.Users;

public interface IInitiateUserRegistrationUseCase
{
    Task<OperationResult<UserResponse>> ExecuteAsync(CreateUserDto userDto);
}
