using Microsoft.AspNetCore.Mvc;
using System.Blog.Application.DTOs;
using System.Blog.Application.Interfaces.Users;

namespace System.Blog.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IGetAllUserUseCase _getAllUserUseCase;
    private readonly ICreateUserUseCase _createUserUseCase;

    public UserController(
        IGetAllUserUseCase getAllUserUseCase,
        ICreateUserUseCase createUserUseCase)
    {
        _getAllUserUseCase = getAllUserUseCase;
        _createUserUseCase = createUserUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        try
        {
            var result = await _getAllUserUseCase.ExecuteAsync();
            return result.StatusCode == 200
                ? Ok(result)
                : StatusCode(result.StatusCode, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error has occurred. - {ex}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] CreateUserDto userDto)
    {
        try
        {
            var result = await _createUserUseCase.ExecuteAsync(userDto);
            return result.StatusCode == 201
                ? Ok(result)
                : StatusCode(result.StatusCode, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error has occurred. - {ex}");
        }
    }
}
