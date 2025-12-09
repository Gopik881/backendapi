using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.AccountHistory
{
    public record GetCompany5TabAccountHistoryQuery(int UserId, int CompanyId, int VersionNumber) : IRequest<Company5TabHistoryDto>;

    public class GetCompany5TabAccountHistoryQueryHandler : IRequestHandler<GetCompany5TabAccountHistoryQuery, Company5TabHistoryDto>
    {
        private readonly IAccountHistoryRepository _accountHistoryRepository;

        public GetCompany5TabAccountHistoryQueryHandler(IAccountHistoryRepository accountHistoryRepository)
        {
            _accountHistoryRepository = accountHistoryRepository;
        }

        public async Task<Company5TabHistoryDto> Handle(GetCompany5TabAccountHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _accountHistoryRepository.GetCompany5TabAccountHistoryByVersionAsync(
                request.UserId,
                request.CompanyId,
                request.VersionNumber
            );
        }
    }
}
