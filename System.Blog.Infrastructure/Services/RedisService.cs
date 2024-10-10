using StackExchange.Redis;
using System.Blog.Core.Contracts.Services;

namespace System.Blog.Infrastructure.Services;

public class RedisService : IRedisService
{
    private readonly IDatabase _redisDb;

    public RedisService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task<string?> GetVerificationCodeAsync(string email)
    {
        var hashEntries = await _redisDb.HashGetAllAsync(email);
        foreach (var entry in hashEntries)
        {
            if (entry.Name == "Code")
            {
                return entry.Value;
            }
        }
        return null;
    }

    public async Task<DateTime?> GetLastSentTimeAsync(string email)
    {
        var hashEntries = await _redisDb.HashGetAllAsync(email);
        foreach (var entry in hashEntries)
        {
            if (entry.Name == "Timestamp")
            {
                return DateTime.Parse(entry.Value);
            }
        }
        return null;
    }

    public async Task SetVerificationCodeAsync(string email, string code)
    {
        await _redisDb.HashSetAsync(email, new[] { new HashEntry("Code", code) });
    }

    public async Task SetLastSentTimeAsync(string email, DateTime time)
    {
        await _redisDb.HashSetAsync(email, new[] { new HashEntry("Timestamp", time.ToString()) });
    }

    public async Task RemoveVerificationCodeAsync(string email)
    {
        await _redisDb.KeyDeleteAsync(email); 
    }
}
