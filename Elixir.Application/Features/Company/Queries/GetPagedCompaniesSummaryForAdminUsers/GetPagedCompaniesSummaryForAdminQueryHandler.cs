using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetPagedCompaniesSummaryForAdminUsers;

public record GetPagedCompaniesSummaryForAdminUsersQuery(int userId,bool IsUnderEdit, bool IsSuperAdmin, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<CompanySummaryDto>>;
public class GetPagedCompaniesSummaryForAdminUsersQueryHandler : IRequestHandler<GetPagedCompaniesSummaryForAdminUsersQuery, PaginatedResponse<CompanySummaryDto>>
{
    private readonly ICompaniesRepository _companiesRepository;
    public GetPagedCompaniesSummaryForAdminUsersQueryHandler(ICompaniesRepository companiesRepository)
    {
        _companiesRepository = companiesRepository;
    }

    public async Task<PaginatedResponse<CompanySummaryDto>> Handle(GetPagedCompaniesSummaryForAdminUsersQuery request, CancellationToken cancellationToken)
    {
        if (request.IsSuperAdmin)
        {
            // If the user is a super admin, fetch all companies summary
            var result = await _companiesRepository.GetPagedSuperAdminCompaniesSummaryAsync(request.userId, request.IsUnderEdit, request.IsSuperAdmin, request.SearchTerm, request.PageNumber, request.PageSize);
            return new PaginatedResponse<CompanySummaryDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        }
        else
        {
            //If the user is a delegate admin, fetch companies summary for delegate admins
            var result = await _companiesRepository.GetPagedDelegateAdminCompaniesSummaryAsync(request.userId, request.IsUnderEdit, request.IsSuperAdmin, request.SearchTerm, request.PageNumber, request.PageSize);
            return new PaginatedResponse<CompanySummaryDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        }
    }
}
