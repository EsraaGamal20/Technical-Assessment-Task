using App.Domain.Common;
using App.Domain.Enums;

namespace App.Domain.Entities;

public class TaskItem : BaseEntity<Guid>
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
