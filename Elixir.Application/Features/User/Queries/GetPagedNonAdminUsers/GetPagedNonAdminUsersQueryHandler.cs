using Elixir.Application.Common.Models;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.User.Queries.GetPagedNonAdminUsers;

public record GetPagedNonAdminUsersQuery(string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<NonAdminUserDto>>;
public class GetPagedNonAdminUsersQueryHandler : IRequestHandler<GetPagedNonAdminUsersQuery, PaginatedResponse<NonAdminUserDto>>
{
    private readonly IUsersRepository _usersRepository;

    public GetPagedNonAdminUsersQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
    public async Task<PaginatedResponse<NonAdminUserDto>> Handle(GetPagedNonAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var result = await _usersRepository.GetFilteredNonAdminUsersAsync(request.SearchTerm, request.PageNumber, request.PageSize);
        // Apply Pagination
        return new PaginatedResponse<NonAdminUserDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));

    }
}
