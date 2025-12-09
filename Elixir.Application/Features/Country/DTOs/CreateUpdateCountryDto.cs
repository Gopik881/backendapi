namespace Elixir.Application.Features.Country.DTOs;

public class CreateUpdateCountryDto
{
    public int? CountryId { get; set; } = 0;
    public string CountryName { get; set; } = null!;
    public string CountryShortName { get; set; } = null!;
    public string? Description { get; set; }
}
