using System.Security.Cryptography;

namespace System.Blog.Application.Utils;

public static class CodeGenerator
{
    public static string GenerateVerificationCode(int length = 6)
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
}
