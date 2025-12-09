
using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.Authentication.Models;

namespace Elixir.Application.Interfaces.Services;

public interface ICryptoService
{
    bool FixedTimeEquals(byte[] left, byte[] right);
    int GenerateIntegerHashForString(string data, bool IgnoreCase = true);
    string GenerateRandomKey();
    string GenerateSalt();
    string GenerateTokenWithStringAndRandomKey(string data, string key);
    byte[]? HashPasswordWithSalt(byte[] password, byte[] salt);
    UserToken GenerateEncryptedToken(string subject, string email, Dictionary<string, string> otherClaims);
    public string EncryptPasswordResetData(string data);
    public PasswordResetTokenDataDto DecryptPasswordResetData(string encryptedToken);
}
