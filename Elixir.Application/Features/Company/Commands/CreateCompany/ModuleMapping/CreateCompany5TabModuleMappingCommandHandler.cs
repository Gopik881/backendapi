using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateCompany5TabModuleMappingCommand(int companyId, int userId, List<Company5TabModuleMappingDto> ModuleMappingsDto) : IRequest<bool>;

public class CreateCompany5TabModuleMappingCommandHandler : IRequestHandler<CreateCompany5TabModuleMappingCommand, bool>
{
    private readonly IModuleMappingHistoryRepository _moduleMappingRepository;

    public CreateCompany5TabModuleMappingCommandHandler(IModuleMappingHistoryRepository moduleMappingRepository)
    {
        _moduleMappingRepository = moduleMappingRepository;
    }

    public async Task<bool> Handle(CreateCompany5TabModuleMappingCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _moduleMappingRepository.Company5TabCreateModuleMappingDataAsync(request.companyId, request.ModuleMappingsDto, request.userId);
    }
}
