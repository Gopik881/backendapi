
using Elixir.Application.Features.SystemPolicies.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.SystemPolicies.Commands.UpdateSystemPolicy;

public record UpdateSystemPolicyCommand(int SystemPolicyId, CreateUpdateSystemPolicyDto UpdateSystemPolicyDto) : IRequest<bool>;
public class UpdateSystemPolicyCommandHandler : IRequestHandler<UpdateSystemPolicyCommand, bool>
{
    private readonly ISystemPoliciesRepository _systemPoliciesRepository;
    public UpdateSystemPolicyCommandHandler(ISystemPoliciesRepository systemPoliciesRepository)
    {
        _systemPoliciesRepository = systemPoliciesRepository;
    }
    public async Task<bool> Handle(UpdateSystemPolicyCommand request, CancellationToken cancellationToken)
    {
        var systemPolicy = await _systemPoliciesRepository.GetSystemPolicyByIdAsync(request.SystemPolicyId);
        if (systemPolicy == null) return false;
        systemPolicy.NoOfSpecialCharacters= request.UpdateSystemPolicyDto.NoOfSpecialCharacters;
        systemPolicy.NoOfUpperCase = request.UpdateSystemPolicyDto.NoOfUpperCase;
        systemPolicy.NoOfLowerCase = request.UpdateSystemPolicyDto.NoOfLowerCase;
        systemPolicy.MinLength = request.UpdateSystemPolicyDto.MinLength;
        systemPolicy.MaxLength = request.UpdateSystemPolicyDto.MaxLength;
        systemPolicy.SpecialCharactersAllowed = request.UpdateSystemPolicyDto.SpecialCharactersAllowed;
        systemPolicy.HistoricalPasswords = request.UpdateSystemPolicyDto.HistoricalPasswords;
        systemPolicy.PasswordValidityDays = request.UpdateSystemPolicyDto.PasswordValidityDays;
        systemPolicy.UnsuccessfulAttempts = request.UpdateSystemPolicyDto.UnsuccessfulAttempts;
        systemPolicy.LockInPeriodInMinutes = request.UpdateSystemPolicyDto.LockInPeriodInMinutes;
        systemPolicy.SessionTimeoutMinutes = request.UpdateSystemPolicyDto.SessionTimeoutMinutes;
        systemPolicy.FileSizeLimitMb = request.UpdateSystemPolicyDto.FileSizeLimitMb;
        return await _systemPoliciesRepository.UpdateSystemPolicyAsync(systemPolicy);
    }
}
