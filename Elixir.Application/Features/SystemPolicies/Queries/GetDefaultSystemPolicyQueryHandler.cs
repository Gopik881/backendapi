using Elixir.Application.Features.SystemPolicies.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.SystemPolicies.Queries;

public record GetDefaultSystemPolicyQuery : IRequest<SystemPolicyDto>;

public class GetDefaultSystemPolicyQueryHandler : IRequestHandler<GetDefaultSystemPolicyQuery, SystemPolicyDto>
{
    private readonly ISystemPoliciesRepository _systemPoliciesRepository;

    public GetDefaultSystemPolicyQueryHandler(ISystemPoliciesRepository systemPoliciesRepository)
    {
        _systemPoliciesRepository = systemPoliciesRepository;
    }
    public Task<SystemPolicyDto> Handle(GetDefaultSystemPolicyQuery request, CancellationToken cancellationToken)
    {
        return _systemPoliciesRepository.GetDefaultSystemPolicyAsync();
    }
}
