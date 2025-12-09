
namespace Elixir.Application.Features.Country.DTOs;

public class CountryDto
{
    public int CountryId { get; set; }
    public string CountryName { get; set; } = null!;
    public string CountryShortName { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime? CreatedOn { get; set; }


}
