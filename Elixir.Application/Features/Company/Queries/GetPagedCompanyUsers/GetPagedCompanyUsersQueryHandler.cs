using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetPagedCompanyUsers;

public record GetPagedCompanyUsersQuery(int CompanyId, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<CompanyUserDto>>;
public class GetPagedCompanyUsersQueryHandler : IRequestHandler<GetPagedCompanyUsersQuery, PaginatedResponse<CompanyUserDto>>
{
    private readonly ICompaniesRepository _companiesRepository;
    public GetPagedCompanyUsersQueryHandler(ICompaniesRepository companiesRepository)
    {
        _companiesRepository = companiesRepository;
    }
    public async Task<PaginatedResponse<CompanyUserDto>> Handle(GetPagedCompanyUsersQuery request, CancellationToken cancellationToken)
    {
        // Implement the logic to get paginated company Users with filters
        var result = await _companiesRepository.GetFilteredCompanyUsersAsync(request.CompanyId, request.SearchTerm, request.PageNumber, request.PageSize);

        // Apply Pagination
        return new PaginatedResponse<CompanyUserDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        
    }
}
