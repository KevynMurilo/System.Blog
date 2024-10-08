using System.Blog.Application.DTOs;
using System.Blog.Application.Utils;
using System.Blog.Core.Contracts.Repositories;
using System.Blog.Core.Entities;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Blog.Application.Interfaces.Users;
using System.Blog.Core.Contracts.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Blog.Application.Responses;

namespace System.Blog.Application.UseCases.Users;

public class InitiateUserRegistrationUseCase : IInitiateUserRegistrationUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public InitiateUserRegistrationUseCase(IUserRepository userRepository, IEmailService emailService, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _cache = cache;
    }

    public async Task<OperationResult<UserResponse>> ExecuteAsync(CreateUserDto userDto)
    {
        if (await _userRepository.GetByEmailAsync(userDto.Email.ToLower()) != null)
            return new OperationResult<UserResponse> { ReqSuccess = false, Message = "Email already registered", StatusCode = 409 };

        User user = MapUserFromDto(userDto);

        if (!await ProcessUserPhotoAsync(userDto, user))
            return new OperationResult<UserResponse> { ReqSuccess = false, Message = "Invalid file type. Only JPG, JPEG, and PNG are allowed.", StatusCode = 400 };

        string verificationCode = CodeGenerator.GenerateVerificationCode();
        _cache.Set(user.Email.ToLower(), (verificationCode, DateTime.UtcNow), TimeSpan.FromMinutes(15));

        await _emailService.SendVerificationEmailAsync(user.Email, verificationCode, user.Name);

        await _userRepository.AddAsync(user);

        return new OperationResult<UserResponse> { Message = "User created successfully. Please verify your email.", StatusCode = 201 };
    }

    private async Task<bool> ProcessUserPhotoAsync(CreateUserDto userDto, User user)
    {
        if (userDto.Photo == null) return true; 

        if (!IsValidFile(userDto.Photo)) return false;

        user.Photo = await SaveFileAsync(userDto.Photo);
        return true;
    }

    private User MapUserFromDto(CreateUserDto userDto)
    {
        return new User
        {
            Name = userDto.Name,
            Email = userDto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            IsActived = false
        };
    }

    private async Task<string> SaveFileAsync(IFormFile photo)
    {
        if (photo.Length > MaxFileSize)
            throw new InvalidOperationException("File size exceeds the maximum limit of 5 MB.");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "UserProfiles");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var image = await Image.LoadAsync(photo.OpenReadStream()))
        {
            image.Mutate(x => x.Resize(300, 300));

            var encoder = new JpegEncoder { Quality = 75 };
            await image.SaveAsync(filePath, encoder);
        }

        return uniqueFileName;
    }

    private bool IsValidFile(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }
}
