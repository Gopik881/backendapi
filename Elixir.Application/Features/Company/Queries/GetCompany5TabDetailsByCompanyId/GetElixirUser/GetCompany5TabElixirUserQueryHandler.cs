using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetElixirUser;

public record GetCompany5TabElixirUserQuery(int CompanyId, bool IsPrevious) : IRequest<List<Company5TabElixirUserDto>>;

public class GetCompany5TabElixirUserQueryHandler : IRequestHandler<GetCompany5TabElixirUserQuery, List<Company5TabElixirUserDto>>
{
    private readonly IElixirUsersHistoryRepository _companyElixirUserHistoryRepository;

    public GetCompany5TabElixirUserQueryHandler(IElixirUsersHistoryRepository companyElixirUserHistoryRepository)
    {
        _companyElixirUserHistoryRepository = companyElixirUserHistoryRepository;
    }

    public async Task<List<Company5TabElixirUserDto>> Handle(GetCompany5TabElixirUserQuery request, CancellationToken cancellationToken)
    {
        return await _companyElixirUserHistoryRepository.GetCompany5TabLatestElixirUsersHistoryAsync(request.CompanyId, request.IsPrevious);
    }
}
