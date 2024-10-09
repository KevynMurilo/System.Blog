using Microsoft.AspNetCore.Mvc;
using System.Blog.Application.DTOs;
using Microsoft.Extensions.Caching.Memory;
using System.Blog.Application.Interfaces.Users.Registration;
using System.Blog.Application.Interfaces.Users.PasswordManagement;
using System.Blog.Application.Interfaces.Users.UserManagement;

namespace System.Blog.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IGetAllUsersUseCase _getAllUserUseCase;
    private readonly IInitiateUserRegistrationUseCase _initiateUserRegistrationUseCase;
    private readonly ICompleteUserRegistrationUseCase _completeUserRegistrationUseCase;
    private readonly IResendVerificationCodeUseCase _resendVerificationCodeUseCase;
    private readonly IForgotPasswordUseCase _forgotPasswordUseCase;
    private readonly IResetPasswordUseCase _resetPasswordUseCase;
    private readonly IMemoryCache _cache;

    public UserController(
        IGetAllUsersUseCase getAllUserUseCase,
        IInitiateUserRegistrationUseCase initiateUserRegistrationUseCase,
        ICompleteUserRegistrationUseCase completeUserRegistrationUseCase,
        IResendVerificationCodeUseCase resendVerificationCodeUseCase,
        IForgotPasswordUseCase forgotPasswordUseCase,
        IResetPasswordUseCase resetPasswordUseCase,
        IMemoryCache cache)
    {
        _getAllUserUseCase = getAllUserUseCase;
        _initiateUserRegistrationUseCase = initiateUserRegistrationUseCase;
        _completeUserRegistrationUseCase = completeUserRegistrationUseCase;
        _resendVerificationCodeUseCase = resendVerificationCodeUseCase;
        _forgotPasswordUseCase = forgotPasswordUseCase;
        _resetPasswordUseCase = resetPasswordUseCase;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var result = await _getAllUserUseCase.ExecuteAsync();
            return StatusCode(result.StatusCode, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error has occurred. - {ex}");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateAsync([FromForm] CreateUserDto userDto)
    {
        try
        {
            var result = await _initiateUserRegistrationUseCase.ExecuteAsync(userDto);
            return StatusCode(result.StatusCode, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error has occurred. - {ex}");
        }
    }

    [HttpPost("complete-registration")]
    public async Task<IActionResult> VerifyEmailAsync(string email, string code)
    {
        try
        {
            var result = await _completeUserRegistrationUseCase.ExecuteAsync(email, code);
            return StatusCode(result.StatusCode, result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error has occurred. - {ex}");
        }
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerificationCodeAsync(string email)
    {
        try
        {
            var result = await _resendVerificationCodeUseCase.ExecuteAsync(email);
            return StatusCode(result.StatusCode, result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error has occurred. - {ex}");
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPasswordAsync(string email)
    {
        try
        {
            var result = await _forgotPasswordUseCase.ExecuteAsync(email);
            return StatusCode(result.StatusCode, result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error has occurred. - {ex}");
        }
    }

    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var result = await _resetPasswordUseCase.ExecuteAsync(resetPasswordDto);
            return StatusCode(result.StatusCode, result.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error has occurred. - {ex}");
        }
    }
}
