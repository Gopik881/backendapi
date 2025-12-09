namespace Elixir.Application.Features.State.DTOs;
public class StateBulkUploadDto
{
    public int RowId { get; set; }
    public int CountryId { get; set; } 
    public string CountryName { get; set; } = null;
    public string StateName { get; set; } = null!;
    public string StateShortName { get; set; } = null!;
    public string? Description { get; set; }
}
