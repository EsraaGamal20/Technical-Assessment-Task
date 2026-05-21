namespace App.Persistence.Repositories;

public sealed class UserRepository
    : GenericRepository<ApplicationUser, Guid>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<ApplicationUser?> GetByPhoneAsync(string phoneNumber, CancellationToken ct = default)
        => Set.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => Set.AsNoTracking().AnyAsync(u => u.Email == email, ct);

    public Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken ct = default)
        => Set.AsNoTracking().AnyAsync(u => u.PhoneNumber == phoneNumber, ct);
}
