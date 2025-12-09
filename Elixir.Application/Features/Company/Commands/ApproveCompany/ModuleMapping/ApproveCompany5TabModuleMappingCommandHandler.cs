using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.ApproveCompany.ModuleMapping;

public record ApproveCompany5TabModuleMappingCommand(int companyId, int userId, List<Company5TabModuleMappingDto> ModuleMappingsDto) : IRequest<bool>;

public class ApproveCompany5TabModuleMappingCommandHandler : IRequestHandler<ApproveCompany5TabModuleMappingCommand, bool>
{
    private readonly IModuleMappingRepository _moduleMappingRepository;

    public ApproveCompany5TabModuleMappingCommandHandler(IModuleMappingRepository moduleMappingRepository)
    {
        _moduleMappingRepository = moduleMappingRepository;
    }

    public async Task<bool> Handle(ApproveCompany5TabModuleMappingCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _moduleMappingRepository.Company5TabApproveModuleMappingDataAsync(request.companyId, request.ModuleMappingsDto, request.userId);
    }
}
