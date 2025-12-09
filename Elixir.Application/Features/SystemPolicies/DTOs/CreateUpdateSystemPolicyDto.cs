namespace Elixir.Application.Features.SystemPolicies.DTOs;
public class CreateUpdateSystemPolicyDto
{
    public int? MaxLength { get; set; }
    public int? MinLength { get; set; }
    public int? NoOfUpperCase { get; set; }
    public int? NoOfLowerCase { get; set; }
    public int? NoOfSpecialCharacters { get; set; }
    public string? SpecialCharactersAllowed { get; set; }
    public int? HistoricalPasswords { get; set; }
    public int? PasswordValidityDays { get; set; }
    public int? UnsuccessfulAttempts { get; set; }
    public int? LockInPeriodInMinutes { get; set; }
    public int? SessionTimeoutMinutes { get; set; }
    public int? FileSizeLimitMb { get; set; }
}
