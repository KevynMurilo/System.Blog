namespace System.Blog.Core.Contracts.Services;

public interface IRedisService
{
    Task<string> GetVerificationCodeAsync(string email);
    Task<DateTime?> GetLastSentTimeAsync(string email);
    Task SetVerificationCodeAsync(string email, string code);
    Task SetLastSentTimeAsync(string email, DateTime time);
    Task RemoveVerificationCodeAsync(string email);
}
