using Auth.Service.Models;
using Microsoft.AspNetCore.Identity;

namespace Auth.Service.Infrastructure.Data
{
    public interface IAuthStore
    {
        Task<List<ApplicationUser>> GatUserAll();
        Task Update(ApplicationUser applicationUser);
        Task<List<IdentityUserRole<string>>> GatIdentityRoleAll();
    }
}
