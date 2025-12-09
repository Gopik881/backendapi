namespace Elixir.Domain.Entities;

public partial class ElixirUser
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int? ClientId { get; set; }

    public int RoleId { get; set; }

    public int CompanyId { get; set; }

    public int UserGroupId { get; set; }

    public int UserId { get; set; }

    public bool IsDeleted { get; set; }

}
