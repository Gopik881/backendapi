using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetTMIAdminDashBoardDetails;

public record GetTMIAdminDashBoardDetailsQuery(int UserId) : IRequest<TmiDashBoardDetailsDto>;

public class GetTMIAdminDashBoardDetailsQueryHandler : IRequestHandler<GetTMIAdminDashBoardDetailsQuery, TmiDashBoardDetailsDto>
{
    private readonly ICompaniesRepository _companyRepository;

    public GetTMIAdminDashBoardDetailsQueryHandler(ICompaniesRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<TmiDashBoardDetailsDto> Handle(GetTMIAdminDashBoardDetailsQuery request, CancellationToken cancellationToken)
    {
        return await _companyRepository.GetTMIAdminDashBoardDetailsAsync(request.UserId);
    }
}