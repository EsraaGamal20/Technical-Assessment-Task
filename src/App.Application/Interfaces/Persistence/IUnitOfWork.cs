namespace App.Application.Interfaces.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository    Users    { get; }
    IProjectRepository Projects { get; }
    ITaskRepository    Tasks    { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
