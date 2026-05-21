using App.Domain.Common;

namespace App.Domain.Entities;

public class ApplicationUser : BaseEntity<Guid>
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string PasswordHash { get; set; }
    public bool IsPhoneVerified { get; set; } = false;
    public bool IsEmailVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
