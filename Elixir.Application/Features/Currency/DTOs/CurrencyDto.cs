namespace Elixir.Application.Features.Currency.DTOs;

public class CurrencyDto
{
    public int CurrencyId { get; set; }
    public int CountryId { get; set; }
    public string CountryName { get; set; } = null;
    public string CurrencyName { get; set; } = null!;
    public string CurrencyShortName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? CreatedOn { get; set; }

}

