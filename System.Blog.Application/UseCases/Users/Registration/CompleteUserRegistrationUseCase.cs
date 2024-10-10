using System.Blog.Core.Contracts.Repositories;
using System.Blog.Core.Contracts.Services;
using System.Blog.Application.Responses;
using System.Blog.Application.Interfaces.Users.Registration;

namespace System.Blog.Application.UseCases.Users.Registration;

public class CompleteUserRegistrationUseCase : ICompleteUserRegistrationUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IRedisService _redisService; 

    public CompleteUserRegistrationUseCase(IUserRepository userRepository, IEmailService emailService, IRedisService redisService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _redisService = redisService;
    }

    public async Task<OperationResult<string>> ExecuteAsync(string email, string code)
    {
        try
        {
            var lowerEmail = email.ToLower();

            var user = await _userRepository.GetByEmailAsync(lowerEmail);
            if (user == null)
                return new OperationResult<string> { Message = "User not found.", StatusCode = 404 };

            var cachedData = await _redisService.GetVerificationCodeAsync(lowerEmail);
            if (cachedData == null)
                return new OperationResult<string> { Message = "Verification code not found or expired. Please request a new verification code.", StatusCode = 400 };

            if (!cachedData.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase))
                return new OperationResult<string> { Message = "The verification code entered is incorrect. Please check the code and try again.", StatusCode = 400 };

            user.IsActived = true;
            await _userRepository.UpdateAsync(user);

            await _redisService.RemoveVerificationCodeAsync(lowerEmail);

            return new OperationResult<string> { Message = "Email successfully verified. Your account has been activated." };
        }
        catch (Exception ex)
        {
            return new OperationResult<string> { ReqSuccess = false, Message = $"Unexpected error: {ex.Message}", StatusCode = 500 };
        }
    }
}
