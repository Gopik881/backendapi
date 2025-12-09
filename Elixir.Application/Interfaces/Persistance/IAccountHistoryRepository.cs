using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface IAccountHistoryRepository
{
    Task<bool> Company5TabCreateAccountDataAsync(int companyId, int userId, Company5TabAccountDto company5TabAccount, CancellationToken cancellationToken = default);

    Task<bool> IsPanExistsAsync(string pan, int companyId);
    Task<bool> IsTanExistsAsync(string tan, int companyId);
    Task<bool> IsGstInExistsAsync(string gstIn, int companyId);
    Task<bool> IsContractIdExistsAsync(string contractId, int companyId);

    Task<Company5TabAccountDto?> GetCompany5TabLatestAccountHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default);
    Task<bool> WithdrawCompany5TabAccountHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default);
    Task<Company5TabHistoryDto> GetCompany5TabAccountHistoryByVersionAsync(int userId,int companyId,int versionNumber);
}