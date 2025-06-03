using Auth.Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Auth.Service.Infrastructure.Data.EntityFramework;

public static class AuthContextDatabaseMigration
{
    public static void MigrateDatabase(this WebApplication webApp)
    {
        //using (var scope = webApp.Services.CreateScope())
        //{
        //    using var authContext = scope.ServiceProvider.GetRequiredService<AuthContext>();
        //    authContext.Database.Migrate();

        //    //    var services = scope.ServiceProvider;
        //    //    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        //    //    try
        //    //    {
        //    //        //Seed Default Users
        //    //        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        //    //        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        //    //        await ApplicationDbContextSeed.SeedEssentialsAsync(userManager, roleManager);
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        var logger = loggerFactory.CreateLogger<Program>();
        //    //        logger.LogError(ex, "An error occurred seeding the DB.");
        //}
        using var scope = webApp.Services.CreateScope();
        using var authContext = scope.ServiceProvider.GetRequiredService<AuthContext>();

        authContext.Database.Migrate();

    }
}
