using Auth.Service.Models;
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
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    //public async Task<User?> VerifyUserLogin(string username, string password) 
    //    => await Users.FirstOrDefaultAsync( u => u.Username == username && u.Password == password);
}
