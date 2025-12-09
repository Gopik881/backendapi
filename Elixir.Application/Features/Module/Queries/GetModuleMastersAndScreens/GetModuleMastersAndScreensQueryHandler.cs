using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Module.Queries.GetModuleMastersAndScreens;

public record GetModuleMastersAndScreensQuery(List<int> ModuleIds, bool IsMaster) : IRequest<List<object>>;

public class GetModuleMastersAndScreensQueryHandler : IRequestHandler<GetModuleMastersAndScreensQuery, List<object>>
{
    private readonly IModulesRepository _moduleRepository;

    public GetModuleMastersAndScreensQueryHandler(IModulesRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public async Task<List<object>> Handle(GetModuleMastersAndScreensQuery request, CancellationToken cancellationToken)
    {
        return await Task.Run(() => _moduleRepository.GetModuleMastersAndScreens(request.ModuleIds, request.IsMaster), cancellationToken);
    }
}