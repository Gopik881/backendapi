using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.WithdrawCompany.ModuleMapping;

public record Company5TabWithDrawModuleMappingCommand(int CompanyId, int UserId) : IRequest<bool>;

public class Company5TabWithDrawModuleMappingCommandHandler : IRequestHandler<Company5TabWithDrawModuleMappingCommand, bool>
{
    private readonly IModuleMappingHistoryRepository _moduleMappingHistoryRepository;

    public Company5TabWithDrawModuleMappingCommandHandler(IModuleMappingHistoryRepository moduleMappingHistoryRepository)
    {
        _moduleMappingHistoryRepository = moduleMappingHistoryRepository;
    }

    public async Task<bool> Handle(Company5TabWithDrawModuleMappingCommand request, CancellationToken cancellationToken)
    {
        return await _moduleMappingHistoryRepository.WithdrawCompany5TabModuleMappingHistoryAsync(request.CompanyId, request.UserId);
    }
}
