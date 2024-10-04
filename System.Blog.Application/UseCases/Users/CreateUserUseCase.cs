using System.Blog.Application.DTOs;
using System.Blog.Core.Contracts.Repositories;
using System.Blog.Application.Interfaces.Users;
using System.Blog.Core.Entities;
using System.Blog.Core.Responses;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace System.Blog.Application.UseCases.Users;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OperationResult<UserResponse>> ExecuteAsync(CreateUserDto userDto)
    {
        try
        {
            var user = new User { Name = userDto.Name, Email = userDto.Email, PasswordHash = userDto.Password };

            if (userDto.Photo != null)
            {
                if (!IsValidFile(userDto.Photo))
                    return new OperationResult<UserResponse> { Message = "Invalid file type. Only JPG, JPEG, and PNG are allowed.", StatusCode = 400 };

                user.PhotoPath = await SaveFileAsync(userDto.Photo);
            }

            await _userRepository.AddAsync(user);

            return new OperationResult<UserResponse> { Message = "User created successfully", StatusCode = 201 };
        }
        catch (Exception ex)
        {
            return new OperationResult<UserResponse> { ReqSuccess = false, Message = $"Unexpected error: {ex.Message}", StatusCode = 500 };
        }
    }

    private async Task<string> SaveFileAsync(IFormFile photo)
    {
        if (photo.Length > MaxFileSize)
            throw new InvalidOperationException("File size exceeds the maximum limit of 5 MB.");
        
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "UserProfiles");
        Directory.CreateDirectory(uploadsFolder);
        var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}");

        using (var image = await Image.LoadAsync(photo.OpenReadStream()))
        {
            image.Mutate(x => x.Resize(300, 300));

            var encoder = new JpegEncoder { Quality = 75 };
            await image.SaveAsync(filePath, encoder);
        }

        return filePath;
    }

    private bool IsValidFile(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }
}
