using Auth.Service.Models;

namespace Auth.Service.Services.IService
{
    // 2. 리프레시 토큰 관리 서비스
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshTokenAsync(string userId, string ipAddress);
        Task<RefreshToken> GetRefreshTokenAsync(string tokenValue);
        Task<bool> ValidateRefreshTokenAsync(string tokenValue);
        Task RevokeRefreshTokenAsync(string tokenValue, string ipAddress, string reason = null);
        Task RevokeAllUserTokensAsync(string userId, string reason = null);
        Task CleanupExpiredTokensAsync();
        Task<bool> IsTokenActiveAsync(string tokenValue);
    }
}
