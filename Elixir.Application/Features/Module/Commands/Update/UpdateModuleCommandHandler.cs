using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Module.Commands.Update;

public record UpdateModuleCommand(ModuleCreateDto UpdateModuleDto) : IRequest<ModuleStructureResponseV2>;

public class UpdateModuleCommandHandler : IRequestHandler<UpdateModuleCommand, ModuleStructureResponseV2>
{
    private readonly IModulesRepository _moduleRepository;

    public UpdateModuleCommandHandler(IModulesRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public async Task<ModuleStructureResponseV2> Handle(UpdateModuleCommand request, CancellationToken cancellationToken)
    {

        return await _moduleRepository.UpdateModuleStructure(request.UpdateModuleDto);
    }
}
