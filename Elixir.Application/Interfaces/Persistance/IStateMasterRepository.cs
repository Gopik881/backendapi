using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;
public interface IStateMasterRepository
{
    Task<int> CreateStateAsync(StateMaster state);
    Task<bool> DeleteStateAsync(int stateId);
    Task<IEnumerable<StateDto>> GetAllStatesAsync();
    Task<StateMaster> GetStateByIdAsync(int stateId);
    Task<bool> UpdateStateAsync(StateMaster state);
    Task<bool> ExistsWithStateNameAsync(string StateName,int CountryId);
    Task<Tuple<List<StateDto>, int>> GetFilteredStatesAsync(string searchTerm, int pageNumber, int pageSize);
    Task<bool> AnyDuplicateStatesExistsAsync(List<CreateUpdateStateDto> states);
    Task<CountryWithStatesDto?> GetCountryByIdWithStatesAsync(int countryId);
    Task<bool> BulkInsertStatesAsync(List<StateBulkUploadDto> states);

    Task<bool> IsDuplicateStateExistsAsync(CreateUpdateStateDto stateDto);
}