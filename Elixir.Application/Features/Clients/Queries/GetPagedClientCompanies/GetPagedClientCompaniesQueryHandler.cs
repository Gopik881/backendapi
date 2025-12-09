using Elixir.Application.Common.Models;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Queries.GetPagedClientCompanies;

public record GetPagedClientCompaniesQuery(int clientId, int PageNumber, int PageSize, string? SearchTerm) : IRequest<PaginatedResponse<CompanyDto>>;

public class GetPagedClientCompaniesQueryHandler : IRequestHandler<GetPagedClientCompaniesQuery, PaginatedResponse<CompanyDto>>
{
    private readonly IClientsRepository _clientRepository;

    public GetPagedClientCompaniesQueryHandler(IClientsRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<PaginatedResponse<CompanyDto>> Handle(GetPagedClientCompaniesQuery request, CancellationToken cancellationToken)
    {
        var result = await _clientRepository.GetFilteredClientCompaniesAsync(request.clientId, request.PageNumber, request.PageSize, request.SearchTerm);
        // result.Item1 is List<ClientCompanyDto>, but we need List<CompanyDto>
        // Flatten all companies from all ClientCompanyDto entries
        var companies = result.Item1
            .Where(cc => cc.Companies != null)
            .SelectMany(cc => cc.Companies)
            .ToList();

        return new PaginatedResponse<CompanyDto>(companies, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
    }
}
