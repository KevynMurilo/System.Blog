using System.Blog.Core.Contracts.Repositories;
using System.Blog.Application.Responses;
using System.Blog.Application.Interfaces.Users.UserManagement;
using System.Blog.Core.Entities;

namespace System.Blog.Application.UseCases.Users.UserManagement;

public class GetAllUsersUseCase : IGetAllUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OperationResult<IEnumerable<UserResponse>>> ExecuteAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            if (!users.Any()) return new OperationResult<IEnumerable<UserResponse>> { Message = "Users not found", StatusCode = 404 };

            var userResponses = MapToUserResponses(users);

            return new OperationResult<IEnumerable<UserResponse>> { Result = userResponses };
        }
        catch (Exception ex)
        {
            return new OperationResult<IEnumerable<UserResponse>> { ReqSuccess = false, Message = $"Unexpected error: {ex.Message}", StatusCode = 500 };
        }
    }

    private IEnumerable<UserResponse> MapToUserResponses(IEnumerable<User> users)
    {
        return users.Select(user => new UserResponse
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedDate = user.CreatedDate,
            UpdatedDate = user.UpdatedDate,
            IsActived = user.IsActived,
            Photo = user.Photo
        }).ToList();
    }
}
