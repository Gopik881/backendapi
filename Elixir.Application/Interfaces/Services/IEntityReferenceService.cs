namespace Elixir.Application.Interfaces.Services;

public interface IEntityReferenceService
{
    Task<bool> HasActiveReferencesAsync(string columnName, int entityId);
    Task<int?> GetActiveReferenceIdAsync(string columnName, int entityId);
}
