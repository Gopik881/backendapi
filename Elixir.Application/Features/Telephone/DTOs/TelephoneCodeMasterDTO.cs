namespace Elixir.Application.Features.Telephone.DTOs;

public class TelephoneCodeMasterDto
{
    public int TelephoneCodeId { get; set; }
    public int CountryId { get; set; }
    public string CountryName { get; set; }
    public string TelephoneCode { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? CreatedOn { get; set; }
}
