using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.WithdrawCompany.Company;

public record Company5TabWithDrawCompanyDataCommand(int CompanyId, int UserId) : IRequest<bool>;

public class Company5TabWithDrawCompanyDataCommandHandler : IRequestHandler<Company5TabWithDrawCompanyDataCommand, bool>
{
    private readonly ICompanyHistoryRepository _companyDataHistoryRepository;

    public Company5TabWithDrawCompanyDataCommandHandler(ICompanyHistoryRepository companyDataHistoryRepository)
    {
        _companyDataHistoryRepository = companyDataHistoryRepository;
    }

    public async Task<bool> Handle(Company5TabWithDrawCompanyDataCommand request, CancellationToken cancellationToken)
    {
        return await _companyDataHistoryRepository.WithdrawCompany5TabCompanyHistoryAsync(request.CompanyId, request.UserId);
    }
}
