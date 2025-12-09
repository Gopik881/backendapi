using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;

public interface ITelephoneCodeMasterRepository
{
    Task<int> CreateTelephoneCodeAsync(TelephoneCodeMaster entity);
    Task<bool> DeleteTelephoneCodeAsync(int id);
    Task<IEnumerable<TelephoneCodeMasterDto>> GetAllTelephoneCodesAsync();
    Task<TelephoneCodeMaster> GetByIdAsync(int id);
    Task<int> UpdateTelephoneAsync(TelephoneCodeMaster entity);
    Task<bool> ExistsWithTelephoneCodeAsync(string TelephoneCode, int CountryId);
    Task<Tuple<List<TelephoneCodeMasterDto>, int>> GetFilteredTelephoneCodesAsync(string searchTerm, int pageNumber, int pageSize);
    Task<bool> AnyDuplicateTelephoneCodesExistsAsync(List<CreateUpdateTelephoneCodeDto> telephoneCodes);
    Task<bool> BulkInsertTelephoneCodesAsync(List<TelephoneCodeBulkUploadDto> telephoneCodes);

    Task<bool> IsDuplicateTelephoneCodeExistsAsync(CreateUpdateTelephoneCodeDto telephoneCode);
}