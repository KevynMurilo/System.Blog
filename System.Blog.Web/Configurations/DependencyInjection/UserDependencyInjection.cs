using System.Blog.Application.UseCases.Users;
using System.Blog.Core.Contracts.Repositories;
using System.Blog.Application.Interfaces.Users;
using System.Blog.Infrastructure.Repositories;

namespace System.Blog.Web.Configurations.DependencyInjection;

public static class UserDependencyInjection
{
    public static IServiceCollection AddUserDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGetAllUserUseCase, GetAllUserUseCase>();
        services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
        return services;
    }
}
