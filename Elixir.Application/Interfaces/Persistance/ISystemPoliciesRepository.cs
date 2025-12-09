using Elixir.Application.Features.SystemPolicies.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;
public interface ISystemPoliciesRepository
{
    Task<SystemPolicyDto> GetDefaultSystemPolicyAsync();
    Task<SystemPolicy> GetSystemPolicyByIdAsync(int systemPolicyId);
    Task<bool> UpdateSystemPolicyAsync(SystemPolicy systemPolicy);
}