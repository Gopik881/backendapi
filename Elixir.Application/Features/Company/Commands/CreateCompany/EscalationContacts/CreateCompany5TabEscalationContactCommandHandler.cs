using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateCompany5TabEscalationContactCommand(int companyId, int userId, List<Company5TabEscalationContactDto> EscalationContactsDto) : IRequest<bool>;

public class CreateCompany5TabEscalationContactCommandHandler : IRequestHandler<CreateCompany5TabEscalationContactCommand, bool>
{
    private readonly IEscalationContactsHistoryRepository _escalationContactsRepository;

    public CreateCompany5TabEscalationContactCommandHandler(IEscalationContactsHistoryRepository escalationContactsRepository)
    {
        _escalationContactsRepository = escalationContactsRepository;
    }

    public async Task<bool> Handle(CreateCompany5TabEscalationContactCommand request, CancellationToken cancellationToken)
    {
        // Save data
       return await _escalationContactsRepository.Company5TabCreateEscalationContactsDataAsync(request.companyId,request.EscalationContactsDto, request.userId);

    }
}
