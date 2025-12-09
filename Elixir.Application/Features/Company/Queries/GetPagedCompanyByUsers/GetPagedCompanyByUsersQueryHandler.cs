using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetPagedCompanyByUsers;

public record GetPagedCompanyByUsersQuery(int UserId, int GroupId, string GroupName, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<CompanyBasicInfoDto>>;
public class GetPagedCompanyByUsersQueryHandler : IRequestHandler<GetPagedCompanyByUsersQuery, PaginatedResponse<CompanyBasicInfoDto>>
{
    private readonly ICompaniesRepository _companiesRepository;
    public GetPagedCompanyByUsersQueryHandler(ICompaniesRepository companiesRepository)
    {
        _companiesRepository = companiesRepository;
    }
    public async Task<PaginatedResponse<CompanyBasicInfoDto>> Handle(GetPagedCompanyByUsersQuery request, CancellationToken cancellationToken)
    {
        var result = await _companiesRepository.GetFilteredCompanyByUsersAsync(request.UserId, request.GroupId, request.GroupName, request.SearchTerm, request.PageNumber, request.PageSize);
        // Apply Pagination
        return new PaginatedResponse<CompanyBasicInfoDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
    }
}
