using System.Blog.Core.Contracts.Repositories;
using System.Blog.Core.Contracts.Services;
using System.Blog.Application.Utils;
using System.Blog.Application.Responses;
using System.Blog.Application.Interfaces.Users.Registration;

namespace System.Blog.Application.UseCases.Users.Registration;

public class ResendVerificationCodeUseCase : IResendVerificationCodeUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IRedisService _redisService; 
    private const int CooldownMinutes = 2;

    public ResendVerificationCodeUseCase(IUserRepository userRepository, IEmailService emailService, IRedisService redisService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _redisService = redisService;
    }

    public async Task<OperationResult<string>> ExecuteAsync(string email)
    {
        try
        {
            var lowerEmail = email.ToLower();
            var user = await _userRepository.GetByEmailAsync(lowerEmail);
            if (user == null)
                return new OperationResult<string> { Message = "User not found.", StatusCode = 404 };

            var verificationCode = await _redisService.GetVerificationCodeAsync(lowerEmail);
            var lastSentTime = await _redisService.GetLastSentTimeAsync(lowerEmail);

            if (lastSentTime.HasValue)
            {
                var timeSinceLastSent = DateTime.UtcNow - lastSentTime.Value;
                if (timeSinceLastSent.TotalMinutes < CooldownMinutes)
                {
                    var waitTime = CooldownMinutes - (int)timeSinceLastSent.TotalMinutes;
                    return new OperationResult<string> { Message = $"Please wait {waitTime} minutes before requesting another code.", StatusCode = 429 };
                }
            }

            var newCode = CodeGenerator.GenerateVerificationCode();

            await _redisService.SetVerificationCodeAsync(lowerEmail, newCode);
            await _redisService.SetLastSentTimeAsync(lowerEmail, DateTime.UtcNow);

            await _emailService.SendVerificationEmailAsync(email, newCode, user.Name);

            return new OperationResult<string> { Message = "Verification code resent successfully.", StatusCode = 200 };

        }
        catch (Exception ex)
        {
            return new OperationResult<string> { ReqSuccess = false, Message = $"Unexpected error: {ex.Message}", StatusCode = 500 };
        }
    }
}
