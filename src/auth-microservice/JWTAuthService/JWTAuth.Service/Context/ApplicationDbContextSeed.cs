using JWTAuth.Service.Constants;
using JWTAuth.Service.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace JWTAuth.Service.Context
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedEssentialsAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Authorization.ERoles.Administrator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Authorization.ERoles.User.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Authorization.ERoles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Authorization.ERoles.Guest.ToString()));

            var defaultUser = new ApplicationUser
            {
                UserName = Authorization.default_username,
                Email = Authorization.default_email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, Authorization.default_password);
                await userManager.AddToRoleAsync(defaultUser, Authorization.default_role.ToString());
            }
        }
    }
}
