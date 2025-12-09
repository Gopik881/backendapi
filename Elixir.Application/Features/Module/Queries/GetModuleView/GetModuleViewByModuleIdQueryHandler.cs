using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Module.Queries.GetModuleView;

public record GetModuleViewByModuleIdQuery(int ModuleId) : IRequest<ModuleDetailsDto>;

public class GetModuleViewByModuleIdQueryHandler : IRequestHandler<GetModuleViewByModuleIdQuery, ModuleDetailsDto>
{
    private readonly IModulesRepository _moduleRepository;

    public GetModuleViewByModuleIdQueryHandler(IModulesRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public async Task<ModuleDetailsDto> Handle(GetModuleViewByModuleIdQuery request, CancellationToken cancellationToken)
    {
        return await _moduleRepository.GetModuleViewAsync(request.ModuleId);
    }
}