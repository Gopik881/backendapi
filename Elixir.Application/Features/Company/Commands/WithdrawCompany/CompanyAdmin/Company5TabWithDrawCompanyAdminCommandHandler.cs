using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.WithdrawCompany.CompanyAdmin;

public record Company5TabWithDrawCompanyAdminCommand(int CompanyId, int UserId) : IRequest<bool>;

public class Company5TabWithDrawCompanyAdminCommandHandler : IRequestHandler<Company5TabWithDrawCompanyAdminCommand, bool>
{
    private readonly ICompanyAdminUsersHistoryRepository _companyAdminHistoryRepository;

    public Company5TabWithDrawCompanyAdminCommandHandler(ICompanyAdminUsersHistoryRepository companyAdminHistoryRepository)
    {
        _companyAdminHistoryRepository = companyAdminHistoryRepository;
    }

    public async Task<bool> Handle(Company5TabWithDrawCompanyAdminCommand request, CancellationToken cancellationToken)
    {
        return await _companyAdminHistoryRepository.WithdrawCompany5TabCompanyAdminHistoryAsync(request.CompanyId, request.UserId);
    }
}
