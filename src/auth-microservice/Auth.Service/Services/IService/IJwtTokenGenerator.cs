using Auth.Service.ApiModels.Requests;
using Auth.Service.ApiModels.Responses;
using Auth.Service.Infrastructure.Data;
using Auth.Service.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.Service.Services.IService
{
    public interface IJwtTokenGenerator
    {
        //Task<JwtSecurityToken> GenerateToken(ApplicationUser applicationUser);
        Task<TokenResponseDto> GetTokenAsync(TokenRequestDto model);
        Task<TokenResponseDto> RefreshTokenAsync(ApplicationUser user, string token);
    }

    
}
