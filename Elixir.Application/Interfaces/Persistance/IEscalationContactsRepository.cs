using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface IEscalationContactsRepository
{
    Task<bool> Company5TabApproveEscalationContactsDataAsync(int companyId, List<Company5TabEscalationContactDto> escalationContact, int userId, CancellationToken cancellationToken = default);
}