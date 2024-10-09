using System.Blog.Application.Responses;

namespace System.Blog.Application.Interfaces.Users.Registration;

public interface ICompleteUserRegistrationUseCase
{
    Task<OperationResult<string>> ExecuteAsync(string email, string code);
}
