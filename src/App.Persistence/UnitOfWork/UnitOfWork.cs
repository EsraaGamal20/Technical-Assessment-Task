namespace App.Persistence.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public IUserRepository Users { get; }
    public IProjectRepository Projects { get; }
    public ITaskRepository Tasks { get; }

    public UnitOfWork(
        AppDbContext context,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        ITaskRepository taskRepository)
    {
        _context = context;
        Users = userRepository;
        Projects = projectRepository;
        Tasks = taskRepository;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is not null) return;
        _currentTransaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is null) return;
        try
        {
            await _context.SaveChangesAsync(ct);
            await _currentTransaction.CommitAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is null) return;
        try
        {
            await _currentTransaction.RollbackAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
        await _context.DisposeAsync();
    }
}
