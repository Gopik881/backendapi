using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.WithdrawCompany.Account
{
    public record Company5TabWithDrawAccountCommand(int CompanyId, int userId) : IRequest<bool>;

    public class Company5TabWithDrawAccountCommandHandler : IRequestHandler<Company5TabWithDrawAccountCommand, bool>
    {
        private readonly IAccountHistoryRepository _accountHistoryRepository;

        public Company5TabWithDrawAccountCommandHandler(IAccountHistoryRepository accountHistoryRepository)
        {
            _accountHistoryRepository = accountHistoryRepository;
        }

        public async Task<bool> Handle(Company5TabWithDrawAccountCommand request, CancellationToken cancellationToken)
        {
            return await _accountHistoryRepository.WithdrawCompany5TabAccountHistoryAsync(request.CompanyId, request.userId);
        }
    }
}
