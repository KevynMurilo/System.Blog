using System.Blog.Application.Responses;

namespace System.Blog.Application.Interfaces.Users.Registration;

public interface IResendVerificationCodeUseCase
{
    Task<OperationResult<string>> ExecuteAsync(string email);
}
