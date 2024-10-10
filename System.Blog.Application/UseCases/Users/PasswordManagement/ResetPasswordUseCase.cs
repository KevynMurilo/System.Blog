using System.Blog.Application.DTOs;
using System.Blog.Core.Contracts.Repositories;
using System.Blog.Core.Contracts.Services;
using System.Blog.Application.Responses;
using System.Blog.Application.Interfaces.Users.PasswordManagement;

namespace System.Blog.Application.UseCases.Users.PasswordManagement;

public class ResetPasswordUseCase : IResetPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRedisService _redisService;

    public ResetPasswordUseCase(IUserRepository userRepository, IRedisService redisService)
    {
        _userRepository = userRepository;
        _redisService = redisService;
    }

    public async Task<OperationResult<string>> ExecuteAsync(ResetPasswordDto request)
    {
        try
        {
            var lowerEmail = request.Email.ToLower();

            var user = await _userRepository.GetByEmailAsync(lowerEmail);
            if (user == null)
                return new OperationResult<string> { Message = "User not found.", StatusCode = 404 };

            var cachedData = await _redisService.GetVerificationCodeAsync(lowerEmail);
            var lastSentTime = await _redisService.GetLastSentTimeAsync(lowerEmail);

            if (cachedData == null || lastSentTime == null)
                return new OperationResult<string> { Message = "Reset request not found.", StatusCode = 404 };

            if (!cachedData.Equals(request.VerificationCode.Trim(), StringComparison.OrdinalIgnoreCase))
                return new OperationResult<string> { Message = "Invalid or expired verification code.", StatusCode = 400 };

            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
                return new OperationResult<string> { Message = "The new password must be different from the current password.", StatusCode = 400 };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _userRepository.UpdateAsync(user);

            await _redisService.RemoveVerificationCodeAsync(lowerEmail);

            return new OperationResult<string> { Message = "Password reset successfully." };
        }
        catch (Exception ex)
        {
            return new OperationResult<string> { ReqSuccess = false, Message = $"Unexpected error: {ex.Message}", StatusCode = 500 };
        }
    }
}
