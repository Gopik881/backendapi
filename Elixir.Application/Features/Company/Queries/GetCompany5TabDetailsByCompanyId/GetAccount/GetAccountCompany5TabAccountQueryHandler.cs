using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetAccount
{
    public record GetAccountCompany5TabAccountQuery(int CompanyId, bool IsPrevious) : IRequest<Company5TabAccountDto>;

    public class GetAccountCompany5TabAccountQueryHandler : IRequestHandler<GetAccountCompany5TabAccountQuery, Company5TabAccountDto>
    {
        private readonly IAccountHistoryRepository _accountHistoryRepository;

        public GetAccountCompany5TabAccountQueryHandler(IAccountHistoryRepository accountHistoryRepository)
        {
            _accountHistoryRepository = accountHistoryRepository;
        }

        public async Task<Company5TabAccountDto> Handle(GetAccountCompany5TabAccountQuery request, CancellationToken cancellationToken)
        {
            return await _accountHistoryRepository.GetCompany5TabLatestAccountHistoryAsync(request.CompanyId, request.IsPrevious);
        }
    }
}
