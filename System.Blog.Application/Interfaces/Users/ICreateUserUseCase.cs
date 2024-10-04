using System.Blog.Application.DTOs;
using System.Blog.Core.Entities;
using System.Blog.Core.Responses;

namespace System.Blog.Application.Interfaces.Users;

public interface ICreateUserUseCase
{
    Task<OperationResult<UserResponse>> ExecuteAsync(CreateUserDto userDto);
}
