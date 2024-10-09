using System.Blog.Application.DTOs;
using System.Blog.Core.Contracts.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Blog.Application.Responses;
using System.Blog.Application.Interfaces.Users.PasswordManagement;

namespace System.Blog.Application.UseCases.Users.PasswordManagement;

public class ResetPasswordUseCase : IResetPasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;

    public ResetPasswordUseCase(IUserRepository userRepository, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _cache = cache;
    }

    public async Task<OperationResult<string>> ExecuteAsync(ResetPasswordDto request)
    {
        var lowerEmail = request.Email.ToLower();

        var user = await _userRepository.GetByEmailAsync(lowerEmail);
        if (user == null)
            return new OperationResult<string> { Message = "User not found.", StatusCode = 404 };

        if (_cache.TryGetValue(lowerEmail, out (string StoredCode, DateTime LastSentTime) data))
        {
            if (!data.StoredCode.Equals(request.VerificationCode.Trim(), StringComparison.OrdinalIgnoreCase))
                return new OperationResult<string> { Message = "Invalid or expired verification code.", StatusCode = 400 };

            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
                return new OperationResult<string> { Message = "The new password must be different from the current password.", StatusCode = 400 };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            _cache.Remove(lowerEmail);

            return new OperationResult<string> { Message = "Password reset successfully.", StatusCode = 200 };
        }

        return new OperationResult<string> { ReqSuccess = false, Message = "Reset request not found.", StatusCode = 404 };
    }
}
