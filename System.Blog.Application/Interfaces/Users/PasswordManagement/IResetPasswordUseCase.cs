using System.Blog.Application.DTOs;
using System.Blog.Application.Responses;

namespace System.Blog.Application.Interfaces.Users.PasswordManagement;

public interface IResetPasswordUseCase
{
    Task<OperationResult<string>> ExecuteAsync(ResetPasswordDto resetPasswordDto);
}
