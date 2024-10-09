using System.Blog.Core.Contracts.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Blog.Core.Contracts.Services;
using System.Blog.Application.Utils;
using System.Blog.Application.Responses;
using System.Blog.Application.Interfaces.Users.Registration;

namespace System.Blog.Application.UseCases.Users.Registration;

public class ResendVerificationCodeUseCase : IResendVerificationCodeUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;
    private const int CooldownMinutes = 2;

    public ResendVerificationCodeUseCase(IUserRepository userRepository, IEmailService emailService, IMemoryCache cache)
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

        if (_cache.TryGetValue(lowerEmail, out (string StoredCode, DateTime LastSentTime) data))
        {
            var timeSinceLastSent = DateTime.UtcNow - data.LastSentTime;
            if (timeSinceLastSent.TotalMinutes < CooldownMinutes)
            {
                return new OperationResult<string> { Message = $"Please wait {CooldownMinutes} minutes before requesting another code.", StatusCode = 429 };
            }
        }

        var newCode = CodeGenerator.GenerateVerificationCode();
        var timestamp = DateTime.UtcNow;

        _cache.Set(lowerEmail, (newCode, timestamp), TimeSpan.FromMinutes(15));

        await _emailService.SendVerificationEmailAsync(email, newCode, user.Name);

        return new OperationResult<string> { Message = "Verification code resent successfully.", StatusCode = 200 };
    }
}
