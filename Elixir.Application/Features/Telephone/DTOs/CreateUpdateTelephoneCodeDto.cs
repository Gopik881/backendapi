namespace Elixir.Application.Features.Telephone.DTOs;

public class CreateUpdateTelephoneCodeDto
{
    public int? TelephoneCodeId { get; set; } = 0;
    public int CountryId { get; set; }
    public string TelephoneCode { get; set; } = null!;
    public string? Description { get; set; }
}
