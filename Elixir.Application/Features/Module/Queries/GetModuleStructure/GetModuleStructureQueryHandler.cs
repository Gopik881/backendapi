using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record GetModuleStructureQuery() : IRequest<List<ModuleStrucureResponseDto>>;

public class GetModuleStructureQueryHandler : IRequestHandler<GetModuleStructureQuery, List<ModuleStrucureResponseDto>>
{
    private readonly IModulesRepository _modulesRepository;

    public GetModuleStructureQueryHandler(IModulesRepository modulesRepository)
    {
        _modulesRepository = modulesRepository;
    }

    public async Task<List<ModuleStrucureResponseDto>> Handle(GetModuleStructureQuery request, CancellationToken cancellationToken)
    {
        return await _modulesRepository.GetModulesWithSubModulesAsync();
    }
}
