using Microsoft.AspNetCore.Mvc;
using System.Blog.Application.DTOs;
using System.Blog.Application.Interfaces.Users;
using Microsoft.Extensions.Caching.Memory;

namespace System.Blog.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IGetAllUserUseCase _getAllUserUseCase;
    private readonly IInitiateUserRegistrationUseCase _initiateUserRegistrationUseCase;
    private readonly ICompleteUserRegistrationUseCase _completeUserRegistrationUseCase;
    private readonly IResendVerificationCodeUseCase _resendVerificationCodeUseCase;
    private readonly IMemoryCache _cache;

    public UserController(
        IGetAllUserUseCase getAllUserUseCase,
        IInitiateUserRegistrationUseCase initiateUserRegistrationUseCase,
        ICompleteUserRegistrationUseCase completeUserRegistrationUseCase,
        IResendVerificationCodeUseCase resendVerificationCodeUseCase,
        IMemoryCache cache)
    {
        _getAllUserUseCase = getAllUserUseCase;
        _initiateUserRegistrationUseCase = initiateUserRegistrationUseCase;
        _completeUserRegistrationUseCase = completeUserRegistrationUseCase;
        _resendVerificationCodeUseCase = resendVerificationCodeUseCase;
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

}
