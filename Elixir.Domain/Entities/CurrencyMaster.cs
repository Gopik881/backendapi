namespace Elixir.Domain.Entities;

public partial class CurrencyMaster
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int CountryId { get; set; }

    public string CurrencyName { get; set; } = null!;

    public string CurrencyShortName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }
    //public int CurrencyId { get; set; }
}
