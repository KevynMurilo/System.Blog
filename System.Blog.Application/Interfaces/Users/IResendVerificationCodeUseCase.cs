using System.Blog.Application.Responses;

namespace System.Blog.Application.Interfaces.Users;

public interface IResendVerificationCodeUseCase
{
    Task<OperationResult<string>> ExecuteAsync(string email);
}
