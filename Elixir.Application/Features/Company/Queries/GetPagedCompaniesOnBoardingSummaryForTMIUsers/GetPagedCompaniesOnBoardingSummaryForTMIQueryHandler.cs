using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetPagedCompaniesOnBoardingSummaryForTMIUsers;

public record GetPagedCompaniesOnBoardingSummaryForTMIUsersQuery(int userId,string SearchTerm, int PageNumber, int PageSize, bool isDashBoard = false) : IRequest<PaginatedResponse<CompanyTMIOnBoardingSummaryDto>>;
public class GetPagedCompaniesOnBoardingSummaryForTMIUsersQueryHandler : IRequestHandler<GetPagedCompaniesOnBoardingSummaryForTMIUsersQuery, PaginatedResponse<CompanyTMIOnBoardingSummaryDto>>
{
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;
    public GetPagedCompaniesOnBoardingSummaryForTMIUsersQueryHandler(ICompanyOnboardingStatusRepository companyOnboardingStatusRepository)
    {
        _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
    }

    public async Task<PaginatedResponse<CompanyTMIOnBoardingSummaryDto>> Handle(GetPagedCompaniesOnBoardingSummaryForTMIUsersQuery request, CancellationToken cancellationToken)
    {
            var result = await _companyOnboardingStatusRepository.GetPagedTMIUsersCompaniesOnBoardingSummaryAsync(request.userId, request.SearchTerm, request.PageNumber, request.PageSize, request.isDashBoard);
            return new PaginatedResponse<CompanyTMIOnBoardingSummaryDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        
    }
}
