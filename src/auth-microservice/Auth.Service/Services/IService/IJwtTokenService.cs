using Auth.Service.Models;
using System.Security.Claims;

namespace Auth.Service.Services.IService
{
    // 1. JWT 토큰 생성/검증 서비스
    public interface IJwtTokenService
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
        ClaimsPrincipal ValidateAccessToken(string accessToken);
        bool IsTokenExpired(string token);
        DateTime GetTokenExpiry(string token);
    }
}
