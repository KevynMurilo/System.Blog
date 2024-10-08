using System.Blog.Application.DTOs;
using System.Blog.Core.Contracts.Repositories;
using System.Blog.Core.Entities;
using System.Blog.Core.Responses;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Security.Cryptography;
using System.Blog.Application.Interfaces.Users;
using System.Blog.Core.Contracts.Services;
using Microsoft.Extensions.Caching.Memory;

namespace System.Blog.Application.UseCases.Users;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache; 
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public CreateUserUseCase(IUserRepository userRepository, IEmailService emailService, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _cache = cache;
    }

    public async Task<OperationResult<UserResponse>> ExecuteAsync(CreateUserDto userDto)
    {
        try
        {
            if (await _userRepository.GetByEmailAsync(userDto.Email.ToLower()) != null)
                return new OperationResult<UserResponse> { Message = "Email already registered", StatusCode = 409 };

            User? user = MapUserFromDto(userDto);

            if (userDto.Photo != null)
            {
                if (!IsValidFile(userDto.Photo))
                    return new OperationResult<UserResponse> { Message = "Invalid file type. Only JPG, JPEG, and PNG are allowed.", StatusCode = 400 };

                user.PhotoPath = await SaveFileAsync(userDto.Photo);
            }

            // Gerar o código de verificação
            var verificationCode = GenerateVerificationCode();

            // Armazenar o código no cache por 15 minutos
            _cache.Set(user.Email.ToLower(), verificationCode, TimeSpan.FromMinutes(15));

            // Enviar o e-mail com o código de verificação
            await _emailService.SendVerificationEmailAsync(user.Email, verificationCode);

            // Salvando usuário no banco de dados (mas não confirmado)
            await _userRepository.AddAsync(user);

            return new OperationResult<UserResponse> { Message = "User created successfully. Please verify your email.", StatusCode = 201 };
        }
        catch (Exception ex)
        {
            return new OperationResult<UserResponse> { ReqSuccess = false, Message = $"Unexpected error: {ex.Message}", StatusCode = 500 };
        }
    }

    private User MapUserFromDto(CreateUserDto userDto)
    {
        return new User
        {
            Name = userDto.Name,
            Email = userDto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
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

    private string GenerateVerificationCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new char[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[length];
            rng.GetBytes(bytes);

            for (int i = 0; i < length; i++)
            {
                random[i] = chars[bytes[i] % chars.Length];
            }
        }
        return new string(random);
    }

    private bool IsValidFile(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }
}
