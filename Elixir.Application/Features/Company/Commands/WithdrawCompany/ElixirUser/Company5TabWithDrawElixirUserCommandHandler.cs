using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.WithdrawCompany.ElixirUser;

public record Company5TabWithDrawElixirUserCommand(int CompanyId, int UserId) : IRequest<bool>;

public class Company5TabWithDrawElixirUserCommandHandler : IRequestHandler<Company5TabWithDrawElixirUserCommand, bool>
{
    private readonly IElixirUsersHistoryRepository _companyElixirUserHistoryRepository;

    public Company5TabWithDrawElixirUserCommandHandler(IElixirUsersHistoryRepository companyElixirUserHistoryRepository)
    {
        _companyElixirUserHistoryRepository = companyElixirUserHistoryRepository;
    }

    public async Task<bool> Handle(Company5TabWithDrawElixirUserCommand request, CancellationToken cancellationToken)
    {
        return await _companyElixirUserHistoryRepository.WithdrawCompany5TabElixirUsersHistoryAsync(request.CompanyId, request.UserId);
    }
}
