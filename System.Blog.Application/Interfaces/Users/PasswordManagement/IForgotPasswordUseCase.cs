using System.Blog.Application.Responses;

namespace System.Blog.Application.Interfaces.Users.PasswordManagement;

public interface IForgotPasswordUseCase
{
    Task<OperationResult<string>> ExecuteAsync(string email);
}
