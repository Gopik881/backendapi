using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.ApproveCompany.Account;
public record ApproveCompany5TabAccountCommand(int companyId, int userId, Company5TabAccountDto Company5TabAccountDto) : IRequest<bool>;
public class ApproveCompany5TabAccountCommandHandler : IRequestHandler<ApproveCompany5TabAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;
    public ApproveCompany5TabAccountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
    public async Task<bool> Handle(ApproveCompany5TabAccountCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _accountRepository.Company5TabApproveAccountInfoAsync(request.companyId, request.userId, request.Company5TabAccountDto);
    }
}
