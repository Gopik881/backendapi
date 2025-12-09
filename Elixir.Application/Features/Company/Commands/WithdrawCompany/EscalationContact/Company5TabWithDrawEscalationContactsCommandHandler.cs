using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.WithdrawCompany.EscalationContact;

public record Company5TabWithDrawEscalationContactsCommand(int CompanyId, int UserId) : IRequest<bool>;

public class Company5TabWithDrawEscalationContactsCommandHandler : IRequestHandler<Company5TabWithDrawEscalationContactsCommand, bool>
{
    private readonly IEscalationContactsHistoryRepository _companyEscalationContactsHistoryRepository;

    public Company5TabWithDrawEscalationContactsCommandHandler(IEscalationContactsHistoryRepository companyEscalationContactsHistoryRepository)
    {
        _companyEscalationContactsHistoryRepository = companyEscalationContactsHistoryRepository;
    }

    public async Task<bool> Handle(Company5TabWithDrawEscalationContactsCommand request, CancellationToken cancellationToken)
    {
        return await _companyEscalationContactsHistoryRepository.WithdrawCompany5TabEscalationContactsHistoryAsync(request.CompanyId, request.UserId);
    }
}
