namespace Elixir.Domain.Entities;

public partial class UserPasswordHistory
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public string PasswordHash { get; set; } = null!;
    public string Salt { get; set; } = null!;

    public bool IsDeleted { get; set; }

}
