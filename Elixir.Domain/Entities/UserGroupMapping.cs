namespace Elixir.Domain.Entities;

public partial class UserGroupMapping
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int UserId { get; set; }

    public int UserGroupId { get; set; }

    public bool? IsEligible { get; set; }

    public bool IsDeleted { get; set; }

}
