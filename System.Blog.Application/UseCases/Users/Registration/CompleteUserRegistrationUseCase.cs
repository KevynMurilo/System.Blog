using System.Blog.Core.Contracts.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Blog.Core.Contracts.Services;
using System.Blog.Application.Responses;
using System.Blog.Application.Interfaces.Users.Registration;

namespace System.Blog.Application.UseCases.Users.Registration;

public class CompleteUserRegistrationUseCase : ICompleteUserRegistrationUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;

    public CompleteUserRegistrationUseCase(IUserRepository userRepository, IEmailService emailService, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _cache = cache;
    }

    public async Task<OperationResult<string>> ExecuteAsync(string email, string code)
    {
        var lowerEmail = email.ToLower();

        if (_cache.TryGetValue(lowerEmail, out (string StoredCode, DateTime LastSentTime) data))
        {
            if (data.StoredCode.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                var user = await _userRepository.GetByEmailAsync(lowerEmail);
                if (user != null)
                {
                    user.IsActived = true;
                    await _userRepository.UpdateAsync(user);
                    _cache.Remove(lowerEmail);
                    return new OperationResult<string> { Message = "Email verified successfully. User activated." };
                }
                return new OperationResult<string> { Message = "User not found.", StatusCode = 404 };
            }
        }

        return new OperationResult<string> { ReqSuccess = false, Message = "Invalid or expired verification code.", StatusCode = 400 };
    }

}
