using Auth.Service.ApiModels.Responses;
using Auth.Service.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace Auth.Service.Services.IService
{
    // 4. 인증 서비스
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(string email, string password, string ipAddress);
        Task<AuthResultDto> RegisterAsync(RegisterRequest request, string ipAddress);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken, string ipAddress);
        Task<bool> LogoutAsync(string refreshToken, string ipAddress);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string email, string newPassword);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<bool> ConfirmEmailAsync(string userId, string token);
    }
}
