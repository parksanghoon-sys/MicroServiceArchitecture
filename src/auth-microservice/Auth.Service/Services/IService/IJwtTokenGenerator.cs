using Auth.Service.ApiModels;
using Auth.Service.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.Service.Services.IService
{
    public interface IJwtTokenGenerator
    {
        //Task<JwtSecurityToken> GenerateToken(ApplicationUser applicationUser);
        Task<TokenResponseDto> GetTokenAsync(TokenRequestDto model);
    }
}
