using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetPagedCompaniesSummaryForTMIUsers;

public record GetPagedCompaniesSummaryForTMIUsersQuery(int userId,bool IsUnderEdit, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<CompanyTMISummaryDto>>;
public class GetPagedCompaniesSummaryForTMIUsersQueryHandler : IRequestHandler<GetPagedCompaniesSummaryForTMIUsersQuery, PaginatedResponse<CompanyTMISummaryDto>>
{
    private readonly ICompaniesRepository _companiesRepository;
    public GetPagedCompaniesSummaryForTMIUsersQueryHandler(ICompaniesRepository companiesRepository)
    {
        _companiesRepository = companiesRepository;
    }

    public async Task<PaginatedResponse<CompanyTMISummaryDto>> Handle(GetPagedCompaniesSummaryForTMIUsersQuery request, CancellationToken cancellationToken)
    {
            var result = await _companiesRepository.GetPagedTMIUsersCompaniesSummaryAsync(request.userId, request.IsUnderEdit, request.SearchTerm, request.PageNumber, request.PageSize);
            return new PaginatedResponse<CompanyTMISummaryDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        
    }
}
