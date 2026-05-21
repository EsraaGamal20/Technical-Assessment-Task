using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using App.Application.Interfaces.Services;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Persistence.Context;

namespace App.Persistence.Seeding;

public static class DatabaseSeeder
{
    private const string DemoEmail    = "demo@app.local";
    private const string DemoPassword = "Demo@1234";
    private const string DemoPhone    = "+201000000000";

    public static async Task SeedAsync(
        AppDbContext    context,
        IPasswordHasher hasher,
        ILogger         logger,
        CancellationToken ct = default)
    {
        if (await context.Users.AnyAsync(u => u.Email == DemoEmail, ct))
        {
            logger.LogInformation("Seed skipped: demo user already exists.");
            return;
        }

        var now    = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var p1Id   = Guid.NewGuid();
        var p2Id   = Guid.NewGuid();

        var user = new ApplicationUser
        {
            Id               = userId,
            FullName         = "Demo User",
            Email            = DemoEmail,
            PhoneNumber      = DemoPhone,
            PasswordHash     = hasher.Hash(DemoPassword),
            IsPhoneVerified  = true,
            IsEmailVerified  = true,
            CreatedAt        = now,
        };

        var projects = new[]
        {
            new Project
            {
                Id          = p1Id,
                Name        = "Website Redesign",
                Description = "Q1 marketing site refresh.",
                OwnerId     = userId,
                CreatedAt   = now,
            },
            new Project
            {
                Id          = p2Id,
                Name        = "Mobile App MVP",
                Description = "iOS + Android first cut.",
                OwnerId     = userId,
                CreatedAt   = now,
            },
        };

        var tasks = new[]
        {
            new TaskItem
            {
                Id          = Guid.NewGuid(),
                Title       = "Wireframe home page",
                Description = "Create low-fidelity wireframes for the landing page.",
                Status      = TaskItemStatus.Done,
                Priority    = TaskPriority.High,
                ProjectId   = p1Id,
                CreatedAt   = now,
            },
            new TaskItem
            {
                Id          = Guid.NewGuid(),
                Title       = "Implement design tokens",
                Description = "Set up colour palette, typography, and spacing in CSS.",
                Status      = TaskItemStatus.InProgress,
                Priority    = TaskPriority.Medium,
                ProjectId   = p1Id,
                CreatedAt   = now,
            },
            new TaskItem
            {
                Id          = Guid.NewGuid(),
                Title       = "Set up CI/CD pipeline",
                Description = "GitHub Actions workflow for build, test, and deploy.",
                Status      = TaskItemStatus.Todo,
                Priority    = TaskPriority.High,
                ProjectId   = p2Id,
                CreatedAt   = now,
            },
            new TaskItem
            {
                Id          = Guid.NewGuid(),
                Title       = "User authentication screens",
                Description = "Login, register, and forgot-password flows.",
                Status      = TaskItemStatus.Todo,
                Priority    = TaskPriority.Critical,
                ProjectId   = p2Id,
                CreatedAt   = now,
            },
        };

        await context.Users.AddAsync(user, ct);
        await context.Projects.AddRangeAsync(projects, ct);
        await context.Tasks.AddRangeAsync(tasks, ct);
        await context.SaveChangesAsync(ct);

        logger.LogInformation(
            "Seed complete. Email={Email} Password={Password} Projects={Projects} Tasks={Tasks}",
            DemoEmail, DemoPassword, projects.Length, tasks.Length);
    }
}
