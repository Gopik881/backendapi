using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record GetAllModulesQuery() : IRequest<IEnumerable<ModuleDto>>;

public class GetAllModulesQueryHandler : IRequestHandler<GetAllModulesQuery, IEnumerable<ModuleDto>>
{
    private readonly IModulesRepository _modulesRepository;

    public GetAllModulesQueryHandler(IModulesRepository modulesRepository)
    {
        _modulesRepository = modulesRepository;
    }

    public async Task<IEnumerable<ModuleDto>> Handle(GetAllModulesQuery request, CancellationToken cancellationToken)
    {
        return await _modulesRepository.GetAllModulesAsync();
    }
}
