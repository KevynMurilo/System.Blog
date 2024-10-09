using System.Blog.Application.Utils;
using System.Blog.Core.Contracts.Repositories;
using System.Blog.Core.Contracts.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Blog.Application.Responses;
using System.Blog.Application.Interfaces.Users.PasswordManagement;

namespace System.Blog.Application.UseCases.Users.PasswordManagement;

public class ForgotPasswordUseCase : IForgotPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;
    private const int TokenValidityMinutes = 15;

    public ForgotPasswordUseCase(IUserRepository userRepository, IEmailService emailService, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _cache = cache;
    }

    public async Task<OperationResult<string>> ExecuteAsync(string email)
    {
        var lowerEmail = email.ToLower();
        var user = await _userRepository.GetByEmailAsync(lowerEmail);
        if (user == null)
            return new OperationResult<string> { Message = "User not found.", StatusCode = 404 };

        var resetCode = CodeGenerator.GenerateVerificationCode();
        var timestamp = DateTime.UtcNow;

        _cache.Set(lowerEmail, (resetCode, timestamp), TimeSpan.FromMinutes(TokenValidityMinutes));

        await _emailService.SendPasswordResetEmailAsync(user.Email, resetCode, user.Name);

        return new OperationResult<string> { Message = "Reset password code sent successfully.", StatusCode = 200 };
    }
}
