using Auth.Service.Models;

namespace Auth.Service.Infrastructure.Data
{
    public interface IAuthStore
    {
        Task<List<ApplicationUser>> GatUserAll();
    }
}
