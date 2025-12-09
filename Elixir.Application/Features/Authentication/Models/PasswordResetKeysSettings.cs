using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Authentication.Models
{
    public class PasswordResetKeysSettings
    {
        public string AESKey { get; set; } = string.Empty;
        public string AESIV { get; set; } = string.Empty;
        public int TokenExpiryInMinutes { get; set; } = 60; // Default expiry time set to 60 minutes
    }
}
