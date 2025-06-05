using Auth.Service.ApiModels.Responses;
using Auth.Service.Models;

namespace Auth.Service.Services.IService
{

    // 3. 통합 토큰 서비스 (JWT + Refresh Token)
    public interface ITokenService
    {
        Task<TokenResponseDto> GenerateTokensAsync(ApplicationUser user, string ipAddress);
        Task<TokenResponseDto> RefreshTokensAsync(string refreshToken, string ipAddress);
        Task<bool> RevokeTokenAsync(string refreshToken, string ipAddress);
        Task RevokeAllUserTokensAsync(string userId);
    }    
}
