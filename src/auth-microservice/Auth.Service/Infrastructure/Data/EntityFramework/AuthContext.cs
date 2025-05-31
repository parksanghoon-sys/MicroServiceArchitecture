using Auth.Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Service.Infrastructure.Data.EntityFramework;

public class AuthContext : IdentityDbContext<ApplicationUser>, IAuthStore
{
    public AuthContext(DbContextOptions<AuthContext> options)
        : base(options)
    {
        
    }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }    

    public async Task<List<IdentityUserRole<string>>> GatIdentityRoleAll()
    {
        return await this.UserRoles.ToListAsync();
    }

    public async Task<List<ApplicationUser>> GatUserAll()
    {
        if(ApplicationUsers is not null)
        {
            return await ApplicationUsers.ToListAsync();
        }
        return default;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthContext).Assembly);
    }

    //public async Task<User?> VerifyUserLogin(string username, string password) 
    //    => await Users.FirstOrDefaultAsync( u => u.Username == username && u.Password == password);
}
public class Authorization
{
    public enum ERoles
    {
        Admin,        
        User
    }
    public const string default_username = "user";
    public const string default_email = "user@users.com";
    public const string default_password = "Password!@123";
    public const ERoles default_role = ERoles.User;
}
public class ApplicationDbContextSeed
{
    public static async Task SeedEssentialsAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {        
        //Seed Roles
        await roleManager.CreateAsync(new IdentityRole(Authorization.ERoles.Admin.ToString()));        
        await roleManager.CreateAsync(new IdentityRole(Authorization.ERoles.User.ToString()));

        //Seed Default User
        var defaultUser = new ApplicationUser { UserName = Authorization.default_username, Email = Authorization.default_email, EmailConfirmed = true, PhoneNumberConfirmed = true };

        if (userManager.Users.All(u => u.Id != defaultUser.Id))
        {
            await userManager.CreateAsync(defaultUser, Authorization.default_password);
            await userManager.AddToRoleAsync(defaultUser, Authorization.default_role.ToString());
        }
    }
}