namespace Elixir.Domain.Entities;

public partial class CountryMaster
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string CountryName { get; set; } = null!;

    public string CountryShortName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

}
