namespace Elixir.Application.Interfaces.Persistance;

public interface ITransactionRepository :IDisposable
{
    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task BeginTransactionAsync();
    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CommitAsync();
    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task RollbackAsync();

}
