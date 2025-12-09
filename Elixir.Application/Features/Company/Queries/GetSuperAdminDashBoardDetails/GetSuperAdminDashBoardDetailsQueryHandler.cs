using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetSuperAdminDashBoardDetails;

public record GetSuperAdminDashBoardDetailsQuery() : IRequest<SuperAdminDashBoardDetailsDto>;

public class GetSuperAdminDashBoardDetailsQueryHandler : IRequestHandler<GetSuperAdminDashBoardDetailsQuery, SuperAdminDashBoardDetailsDto>
{
    private readonly ICompaniesRepository _companyRepository;

    public GetSuperAdminDashBoardDetailsQueryHandler(ICompaniesRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<SuperAdminDashBoardDetailsDto> Handle(GetSuperAdminDashBoardDetailsQuery request, CancellationToken cancellationToken)
    {
        return await _companyRepository.GetSuperAdminDashBoardDetailsAsync();
    }
}
