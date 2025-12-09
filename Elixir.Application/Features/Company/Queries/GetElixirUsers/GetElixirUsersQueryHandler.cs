using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetElixirUsers;

public record GetElixirUsersQuery(int CompanyId) : IRequest<ElixirUserListDto>;

public class GetElixirUsersQueryHandler : IRequestHandler<GetElixirUsersQuery, ElixirUserListDto>
{
    private readonly IElixirUsersRepository _elixirUserRepository;

    public GetElixirUsersQueryHandler(IElixirUsersRepository elixirUserRepository)
    {
        _elixirUserRepository = elixirUserRepository;
    }

    public async Task<ElixirUserListDto> Handle(GetElixirUsersQuery request, CancellationToken cancellationToken)
    {
        return await _elixirUserRepository.GetElixirUserListsByCompanyIdAsync(request.CompanyId);
    }
}
