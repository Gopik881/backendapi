using Elixir.Application.Interfaces.Persistance;
using MediatR;


namespace Elixir.Application.Features.Company.Queries.CheckCompanyCode;

public record CheckCompanyCodeExistsQuery(string CompanyCode) : IRequest<int>;

public class CheckCompanyCodeExistsQueryHandler : IRequestHandler<CheckCompanyCodeExistsQuery, int>
{
    private readonly ICompaniesRepository _companiesRepository;
    public CheckCompanyCodeExistsQueryHandler(ICompaniesRepository companiesRepository)
    {
        _companiesRepository = companiesRepository;
    }
    public async Task<int> Handle(CheckCompanyCodeExistsQuery request, CancellationToken cancellationToken)
    {
        //Return 0 if the no company code exists, otherwise return Company Code
        return await _companiesRepository.FindCompanyByCodeAsync(request.CompanyCode);
    }
}
