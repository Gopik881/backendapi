namespace Elixir.Domain.Entities;

public partial class TelephoneCodeMaster
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int CountryId { get; set; }

    public string TelephoneCode { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

}
