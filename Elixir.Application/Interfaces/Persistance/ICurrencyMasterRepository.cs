using Elixir.Application.Features.Currency.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;

public interface ICurrencyMasterRepository
{
    Task<int> CreateCurrencyAsync(CurrencyMaster currency);
    Task<bool> DeleteAsync(int currencyId);
    Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
    Task<CurrencyMaster> GetCurrencyByIdAsync(int currencyId);
    Task<bool> UpdateAsync(CurrencyMaster currency);
    Task<bool> ExistsWithCurrencyNameAsync(string CurrencyName, int CountryId);
    Task<Tuple<List<CurrencyDto>, int>> GetFilteredCurrenciesAsync(string searchTerm, int pageNumber, int pageSize);
    Task<bool> AnyDuplicateCurrenciesExistsAsync(List<CreateUpdateCurrencyDto> currencies);
    Task<bool> BulkInsertCurrenciesAsync(List<CurrencyBulkUploadDto> currencies);
    Task<bool> IsDuplicateCurrencyExistsAsync(CreateUpdateCurrencyDto currency);
}