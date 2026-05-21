namespace App.Persistence.Context;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Supports running from solution root (src/App.Api) or from the
        // App.Persistence project dir (../App.Api).
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "src", "App.Api"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "App.Api"),
            Directory.GetCurrentDirectory(),
        };

        var basePath = candidates.FirstOrDefault(Directory.Exists)
            ?? Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetFullPath(basePath))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseSqlServer(connectionString, sql =>
            sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

        return new AppDbContext(builder.Options);
    }
}
