using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateCompany5TabAccountCommand(int companyId, int userId, Company5TabAccountDto Company5TabAccountDto) : IRequest<bool>;

public class CreateCompany5TabAccountCommandHandler : IRequestHandler<CreateCompany5TabAccountCommand, bool>
{
    private readonly IAccountHistoryRepository _accountHistoryRepository;
    public CreateCompany5TabAccountCommandHandler(IAccountHistoryRepository accountHistoryRepository)
    {
        _accountHistoryRepository = accountHistoryRepository;
    }

    public async Task<bool> Handle(CreateCompany5TabAccountCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _accountHistoryRepository.Company5TabCreateAccountDataAsync(request.companyId, request.userId, request.Company5TabAccountDto);
    }
}
