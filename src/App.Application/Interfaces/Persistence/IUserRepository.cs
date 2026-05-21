using App.Domain.Entities;

namespace App.Application.Interfaces.Persistence;

public interface IUserRepository : IGenericRepository<ApplicationUser, Guid>
{
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<ApplicationUser?> GetByPhoneAsync(string phoneNumber, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken ct = default);
}
