using Elixir.Application.Features.Country.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;

public interface ICountryMasterRepository
{
    Task<bool> ExistsAsync(int countryId);
    Task<bool> ExistsWithCountryNameAsync(string countryName);
    Task<Tuple<List<CountryWithStatesDto>, int>> GetCountryByIdWithStatesAsync(string searchTerm, int pageNumber, int pageSize, int countryId);
    Task<int> CreateCountryAsync(CountryMaster country);
    Task<bool> DeleteCountryAsync(int countryId);
    Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
    Task<CountryMaster> GetCountryByIdAsync(int countryId);
    Task<bool> UpdateCountryAsync(CountryMaster country);
    Task<Tuple<List<CountryDto>, int>> GetFilteredCountriesAsync(string searchTerm, int pageNumber,int pageSize);
    Task<bool> BulkInsertCountriesAsync(List<CountryBulkUploadDto> countries);
    Task<bool> AnyDuplicateDuplicateCountrysExistsAsync(List<CreateUpdateCountryDto> countries);

    Task<int?> GetBulkUploadFileSizeLimitMbAsync();
    Task<bool> IsDuplicateCountryExistsAsync(CreateUpdateCountryDto country);
}