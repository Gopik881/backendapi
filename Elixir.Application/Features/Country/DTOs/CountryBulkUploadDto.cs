namespace Elixir.Application.Features.Country.DTOs;

public class CountryBulkUploadDto
{
    public int RowId { get; set; }
    public string CountryName { get; set; } = null!;
    public string CountryShortName { get; set; } = null!;
    public string? Description { get; set; }

}
