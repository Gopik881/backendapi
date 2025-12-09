using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Module.Queries.GetModuleSubmoduleList;

public record GetModuleSubModuleListQuery(List<int> ModuleIds) : IRequest<List<object>>;

public class GetModuleSubModuleListQueryHandler : IRequestHandler<GetModuleSubModuleListQuery, List<object>>
{
    private readonly IModulesRepository _moduleRepository;

    public GetModuleSubModuleListQueryHandler(IModulesRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public async Task<List<object>> Handle(GetModuleSubModuleListQuery request, CancellationToken cancellationToken)
    {
        return await _moduleRepository.GetModuleSubmoduleListAsync(request.ModuleIds);
    }
}