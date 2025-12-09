using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.Authentication.Models;
using Elixir.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Elixir.Infrastructure.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly RSA _rsa;
        private readonly JwtSettings _jwtSettings;
        private readonly PasswordResetKeysSettings _prkSettings;

        private int Length { get; } = 128 / 4;

        public CryptoService(RSA rsa, IOptions<JwtSettings> jwtOptions, IOptions<PasswordResetKeysSettings> prkOptions)
        {
            _jwtSettings = jwtOptions.Value;
            _rsa = rsa;
            _prkSettings = prkOptions.Value;
        }

        public string GenerateSalt()
        {
            var randomData = new byte[Length];
            RandomNumberGenerator.Fill(randomData);
            return Convert.ToBase64String(randomData);
        }
        /// <summary>
        /// HashPasswordWithSalt provides Hash for Password with Salt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>Password Hash</returns>
        public byte[]? HashPasswordWithSalt(byte[] password, byte[] salt)
        {
            byte[]? hash;
            using (SHA256 sha = SHA256.Create())
            {
                sha.TransformBlock(salt, 0, salt.Length, salt, 0);
                sha.TransformFinalBlock(password, 0, password.Length);
                hash = sha.Hash;
            }
            return hash;
        }

        /// <summary>
        /// GenerateIntegerHashForString - Generate a Hash for given string
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Hash of the Stirng in integer format</returns>
        public int GenerateIntegerHashForString(string data, bool IgnoreCase = true)
        {
            if (IgnoreCase)
                data = data.ToLowerInvariant(); // Use `ToLowerInvariant()` for better performance & consistency

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Combine values using XOR from different positions for better bit distribution
                return BitConverter.ToInt32(hashedBytes, 0) ^
                       BitConverter.ToInt32(hashedBytes, 8) ^
                       BitConverter.ToInt32(hashedBytes, 24);
            }

        }

        /// <summary>
        /// GenerateRandomKey - Generate new Random Key without hypen
        /// </summary>
        /// <returns></returns>
        public string GenerateRandomKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// GenerateTokenWithStringAndRandomKey - Combines two strings and generates a Token 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GenerateTokenWithStringAndRandomKey(string data, string key)
        {
            byte[] input1 = Encoding.UTF8.GetBytes(data);
            byte[] input2 = Encoding.UTF8.GetBytes(key);
            byte[] hash;
            using (SHA256 sha = SHA256.Create())
            {
                sha.TransformBlock(input1, 0, input1.Length, input1, 0);
                sha.TransformFinalBlock(input2, 0, input2.Length);
                hash = sha.Hash != null ? sha.Hash : Array.Empty<byte>();
            }
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// FixedTimeEquals - Compares two byte array in fixed time and return true or false
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>bool</returns>
        public bool FixedTimeEquals(byte[] left, byte[] right)
        {
            // NoOptimization because we want this method to be exactly as non-short-circuiting as written.
            // NoInlining because the NoOptimization would get lost if the method got inlined.
            if (left.Length != right.Length)
                return false;
            int length = left.Length;
            int accum = 0;
            for (int i = 0; i < length; i++)
            {
                accum |= left[i] - right[i];
            }
            return accum == 0;
        }

        public UserToken GenerateEncryptedToken(string subject, string email, Dictionary<string, string> otherClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var credentials = new EncryptingCredentials(
                new RsaSecurityKey(_rsa), SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512
            );
            var ClaimsIdentity = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Email, email),
                        new Claim(JwtRegisteredClaimNames.Sub, subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    });
            if (otherClaims != null && otherClaims.Count > 0)
            {
                foreach (var kvp in otherClaims)
                {
                    ClaimsIdentity.AddClaim(new Claim(kvp.Key, kvp.Value));
                }
            }
            if (!string.IsNullOrEmpty(_jwtSettings.Issuer) && !String.IsNullOrEmpty(_jwtSettings.Audience) && !String.IsNullOrEmpty(_jwtSettings.SecretKey))
            {
                var tokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = ClaimsIdentity,
                    Audience = _jwtSettings.Audience,
                    Issuer = _jwtSettings.Issuer,
                    Expires = tokenExpiration,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.SecretKey)), SecurityAlgorithms.HmacSha256Signature),
                    EncryptingCredentials = credentials,
                };
                return new UserToken() { AccessToken = tokenHandler.CreateEncodedJwt(tokenDescriptor), AccessTokenExpiry = tokenExpiration };
            }
            return null;
        }

        public string EncryptPasswordResetData(string data)
        {
            var tokenData = new PasswordResetTokenDataDto
            {
                Email = data,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_prkSettings.TokenExpiryInMinutes)
            };
            string json = JsonSerializer.Serialize(tokenData);
            byte[] key = Convert.FromBase64String(_prkSettings.AESKey); // 32-byte key
            byte[] iv = Convert.FromBase64String(_prkSettings.AESIV); // 16-byte IV

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;

            using var encryptor = aes.CreateEncryptor();
            byte[] encryptedBytes = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(json), 0, json.Length);

            return Convert.ToBase64String(encryptedBytes); // Token ready for reset link
        }
        public PasswordResetTokenDataDto DecryptPasswordResetData(string encryptedToken)
        {
            try
            {
                byte[] key = Convert.FromBase64String(_prkSettings.AESKey); // 32-byte key
                byte[] iv = Convert.FromBase64String(_prkSettings.AESIV); // 16-byte IV

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using var decryptor = aes.CreateDecryptor();
                byte[] encryptedBytes = Convert.FromBase64String(encryptedToken);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return JsonSerializer.Deserialize<PasswordResetTokenDataDto>(Encoding.UTF8.GetString(decryptedBytes));
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid or tampered token.");
            }

        }
    }
}
