using System.Blog.Core.Contracts.Repositories;
using System.Blog.Infrastructure.Repositories;
using System.Blog.Core.Contracts.Services;
using System.Blog.Infrastructure.Services;
using System.Blog.Application.UseCases.Users.Registration;
using System.Blog.Application.UseCases.Users.PasswordManagement;
using System.Blog.Application.UseCases.Users.UserManagement;
using System.Blog.Application.Interfaces.Users.Registration;
using System.Blog.Application.Interfaces.Users.PasswordManagement;
using System.Blog.Application.Interfaces.Users.UserManagement;

namespace System.Blog.Web.Configurations.DependencyInjection;

public static class UserDependencyInjection
{
    public static IServiceCollection AddUserDependencies(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IGetAllUsersUseCase, GetAllUsersUseCase>();
        services.AddScoped<IInitiateUserRegistrationUseCase, InitiateUserRegistrationUseCase>();
        services.AddScoped<IResendVerificationCodeUseCase, ResendVerificationCodeUseCase>();
        services.AddScoped<ICompleteUserRegistrationUseCase, CompleteUserRegistrationUseCase>();
        services.AddScoped<IForgotPasswordUseCase, ForgotPasswordUseCase>();
        services.AddScoped<IResetPasswordUseCase, ResetPasswordUseCase>();
        return services;
    }
}
