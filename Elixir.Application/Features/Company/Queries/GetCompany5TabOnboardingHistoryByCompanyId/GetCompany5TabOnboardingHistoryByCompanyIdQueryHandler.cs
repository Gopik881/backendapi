using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabOnboardingHistoryByCompanyId
{
    public record GetCompany5TabOnboardingHistoryByCompanyIdQuery(int CompanyId) : IRequest<IEnumerable<Company5TabOnboardingHistoryDto>>;

    public class GetCompany5TabOnboardingHistoryByCompanyIdQueryHandler : IRequestHandler<GetCompany5TabOnboardingHistoryByCompanyIdQuery, IEnumerable<Company5TabOnboardingHistoryDto>>
    {
        private readonly ICompany5TabOnboardingHistoryRepository _company5TabOnboardingHistoryRepository;

        public GetCompany5TabOnboardingHistoryByCompanyIdQueryHandler(ICompany5TabOnboardingHistoryRepository company5TabOnboardingHistoryRepository)
        {
            _company5TabOnboardingHistoryRepository = company5TabOnboardingHistoryRepository;
        }

        public async Task<IEnumerable<Company5TabOnboardingHistoryDto>> Handle(GetCompany5TabOnboardingHistoryByCompanyIdQuery request, CancellationToken cancellationToken)
        {
            return await _company5TabOnboardingHistoryRepository.GetCompany5TabOnboardingHistoryByCompanyIdAsync(request.CompanyId);
        }
    }
}
