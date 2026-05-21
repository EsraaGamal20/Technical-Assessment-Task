using App.Domain.Common;

namespace App.Domain.Entities;

public class Project : BaseEntity<Guid>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public ApplicationUser Owner { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
