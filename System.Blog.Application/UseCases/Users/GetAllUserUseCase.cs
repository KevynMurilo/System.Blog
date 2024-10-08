using System.Blog.Core.Contracts.Repositories;
using System.Blog.Application.Interfaces.Users;
using System.Blog.Application.Responses;

namespace System.Blog.Application.UseCases.Users;

public class GetAllUserUseCase : IGetAllUserUseCase
{
    private readonly IUserRepository _userRepository;

    public GetAllUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OperationResult<IEnumerable<UserResponse>>> ExecuteAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            if (!users.Any()) return new OperationResult<IEnumerable<UserResponse>> { Message = "Users not found", StatusCode = 404 };

            var userResponses = users.Select(user => new UserResponse
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

            return new OperationResult<IEnumerable<UserResponse>> { Result = userResponses };
        }
        catch (Exception ex)
        {
            return new OperationResult<IEnumerable<UserResponse>> { ReqSuccess = false, Message = $"Unexpected error: {ex.Message}", StatusCode = 500 };
        }
    }
}
