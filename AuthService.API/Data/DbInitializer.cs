﻿using System.Diagnostics;
using AuthService.API.Models;

namespace AuthService.API.Data;

public class DbInitializer(
    IWebHostEnvironment env,
    IServiceProvider serviceProvider,
    ILogger<DbInitializer> logger
) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    private readonly ActivitySource m_ActivitySource = new(ActivitySourceName);
    private ApplicationDbContext m_DbContext = null!;
    private UserManager<ApplicationUser> m_UserManager = null!;
    private RoleManager<IdentityRole> m_RoleManager = null!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        m_DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        m_UserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        m_RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await InitializeDatabaseAsync(cancellationToken);
    }

    private async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        using var activity = m_ActivitySource.StartActivity(ActivityKind.Client);

        var sw = Stopwatch.StartNew();

        var strategy = m_DbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(m_DbContext.Database.MigrateAsync, cancellationToken);

        await SeedAsync(cancellationToken);

        logger.LogInformation("System Database initialization completed after {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);

        logger.LogInformation("Plugin Database System initialization completed after {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);
    }

    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        if (env.IsDevelopment())
        {
        }

        await m_DbContext.SaveChangesAsync(cancellationToken);
    }
}