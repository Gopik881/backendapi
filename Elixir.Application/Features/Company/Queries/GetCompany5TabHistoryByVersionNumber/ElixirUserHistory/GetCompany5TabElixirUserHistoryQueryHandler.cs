using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.ElixirUserHistory;

public record GetCompany5TabElixirUserHistoryQuery(int UserId, int CompanyId, int VersionNumber) : IRequest<Company5TabHistoryDto>;

public class GetCompany5TabElixirUserHistoryQueryHandler : IRequestHandler<GetCompany5TabElixirUserHistoryQuery, Company5TabHistoryDto>
{
    private readonly IElixirUsersHistoryRepository _elixirUsersHistoryRepository;

    public GetCompany5TabElixirUserHistoryQueryHandler(IElixirUsersHistoryRepository elixirUsersHistoryRepository)
    {
        _elixirUsersHistoryRepository = elixirUsersHistoryRepository;
    }

    public async Task<Company5TabHistoryDto> Handle(GetCompany5TabElixirUserHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _elixirUsersHistoryRepository.GetCompany5TabElixirUsersHistoryByVersionAsync(request.UserId, request.CompanyId, request.VersionNumber);
    }
}
