
namespace Elixir.Application.Features.User.DTOs
{
    public class UserPasswordHistoryDataDto
    {
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;

    }
}
