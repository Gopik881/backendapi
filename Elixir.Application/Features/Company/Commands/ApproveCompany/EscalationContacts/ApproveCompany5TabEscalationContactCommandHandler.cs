using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.ApproveCompany.EscalationContacts;

public record ApproveCompany5TabEscalationContactCommand(int companyId, int userId, List<Company5TabEscalationContactDto> EscalationContactsDto) : IRequest<bool>;

public class ApproveCompany5TabEscalationContactCommandHandler : IRequestHandler<ApproveCompany5TabEscalationContactCommand, bool>
{
    private readonly IEscalationContactsRepository _escalationContactsRepository;

    public ApproveCompany5TabEscalationContactCommandHandler(IEscalationContactsRepository escalationContactsRepository)
    {
        _escalationContactsRepository = escalationContactsRepository;
    }

    public async Task<bool> Handle(ApproveCompany5TabEscalationContactCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _escalationContactsRepository.Company5TabApproveEscalationContactsDataAsync(request.companyId, request.EscalationContactsDto, request.userId);

    }
}
