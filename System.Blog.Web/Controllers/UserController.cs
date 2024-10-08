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
    private readonly ICreateUserUseCase _createUserUseCase;
    private readonly IMemoryCache _cache;

    public UserController(
        IGetAllUserUseCase getAllUserUseCase,
        ICreateUserUseCase createUserUseCase,
        IMemoryCache cache)
    {
        _getAllUserUseCase = getAllUserUseCase;
        _createUserUseCase = createUserUseCase;
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
            var result = await _createUserUseCase.ExecuteAsync(userDto);
            return StatusCode(result.StatusCode, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error has occurred. - {ex}");
        }
    }

    [HttpPost("verify-email")]
    public IActionResult VerifyEmail(string email, string code)
    {
        if (_cache.TryGetValue(email, out string? storedCode))
        {
            if (storedCode.Equals(code, StringComparison.OrdinalIgnoreCase))
            {
                _cache.Remove(email);
                return Ok("Email verified successfully.");
            }
        }
        return BadRequest("Invalid or expired verification code.");
    }
}
