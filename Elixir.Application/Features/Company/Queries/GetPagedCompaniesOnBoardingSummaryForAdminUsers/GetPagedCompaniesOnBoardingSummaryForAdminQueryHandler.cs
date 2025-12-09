using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetPagedCompaniesOnBoardingSummaryForAdminUsers;

public record GetPagedCompaniesOnBoardingSummaryForAdminUsersQuery(int userId,bool IsSuperAdmin, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<CompanyOnBoardingSummaryDto>>;
public class GetPagedCompaniesOnBoardingSummaryForAdminUsersQueryHandler : IRequestHandler<GetPagedCompaniesOnBoardingSummaryForAdminUsersQuery, PaginatedResponse<CompanyOnBoardingSummaryDto>>
{
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;
    public GetPagedCompaniesOnBoardingSummaryForAdminUsersQueryHandler(ICompanyOnboardingStatusRepository companyOnboardingStatusRepository)
    {
        _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
    }

    public async Task<PaginatedResponse<CompanyOnBoardingSummaryDto>> Handle(GetPagedCompaniesOnBoardingSummaryForAdminUsersQuery request, CancellationToken cancellationToken)
    {
        if (request.IsSuperAdmin)
        {
            // If the user is a super admin, fetch all companies summary
            var result = await _companyOnboardingStatusRepository.GetPagedSuperAdminCompaniesOnBoardingSummaryAsync(request.userId, request.IsSuperAdmin, request.SearchTerm, request.PageNumber, request.PageSize);
            return new PaginatedResponse<CompanyOnBoardingSummaryDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        }
        else
        {
            //If the user is a delegate admin, fetch companies summary for delegate admins
            var result = await _companyOnboardingStatusRepository.GetPagedDelegateAdminCompaniesOnBoardingSummaryAsync(request.userId, request.SearchTerm, request.PageNumber, request.PageSize);
            return new PaginatedResponse<CompanyOnBoardingSummaryDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        }
    }
}
