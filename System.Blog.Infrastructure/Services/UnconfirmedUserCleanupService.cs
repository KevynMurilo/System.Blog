using System.Blog.Core.Contracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace System.Blog.Infrastructure.Services;

public class UnconfirmedUserCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UnconfirmedUserCleanupService> _logger;
    private const int ExpirationHours = 24; 
    private const string UploadFolder = "Uploads/UserProfiles";

    public UnconfirmedUserCleanupService(IServiceScopeFactory serviceScopeFactory, ILogger<UnconfirmedUserCleanupService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                    var expirationDate = DateTime.UtcNow.AddHours(-ExpirationHours);
                    var unconfirmedUsers = await userRepository.GetUnconfirmedUsersAsync(expirationDate);

                    foreach (var user in unconfirmedUsers)
                    {
                        if (!string.IsNullOrEmpty(user.Photo))
                        {
                            var photoPath = Path.Combine(Directory.GetCurrentDirectory(), UploadFolder, user.Photo);
                            if (File.Exists(photoPath))
                            {
                                File.Delete(photoPath);
                            }
                        }

                        await userRepository.DeleteAsync(user.UserId);
                        _logger.LogInformation($"Deleted unconfirmed user {user.Email} and their photo.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during unconfirmed user cleanup: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
