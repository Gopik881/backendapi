using Elixir.Application.Features.Company.DTOs;
namespace Elixir.Application.Interfaces.Persistance;
public interface IEscalationContactsHistoryRepository
{
    Task<bool> Company5TabCreateEscalationContactsDataAsync(int companyId, List<Company5TabEscalationContactDto> escalationContact, int userId, CancellationToken cancellationToken = default);
    Task<List<Company5TabEscalationContactDto>?> GetCompany5TabLatestEscalationContactsHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default);
    Task<bool> WithdrawCompany5TabEscalationContactsHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default);
    Task<Company5TabHistoryDto> GetCompany5TabEscalationContactsHistoryByVersionAsync(int userId,int companyId,int versionNumber);
}