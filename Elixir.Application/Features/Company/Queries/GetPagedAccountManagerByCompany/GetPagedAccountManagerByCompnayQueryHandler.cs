
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetPagedAccountManagerByCompany;


public record GetPagedAccountManagerByCompnayQuery(int CompanyId, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<CompanyUserDto>>;

public class GetPagedAccountManagerByCompnayQueryHandler : IRequestHandler<GetPagedAccountManagerByCompnayQuery, PaginatedResponse<CompanyUserDto>>
{
    IElixirUsersRepository  _elixirUsersRepository;
    public GetPagedAccountManagerByCompnayQueryHandler(IElixirUsersRepository elixirUsersRepository)
    {
        _elixirUsersRepository = elixirUsersRepository;
    }
    public async Task<PaginatedResponse<CompanyUserDto>> Handle(GetPagedAccountManagerByCompnayQuery request, CancellationToken cancellationToken)
    {
        var result = await _elixirUsersRepository.GetFilteredAccountManagersAsync(request.CompanyId, request.SearchTerm, request.PageNumber, request.PageSize);
        // Apply Pagination
        return new PaginatedResponse<CompanyUserDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));

    }
}
