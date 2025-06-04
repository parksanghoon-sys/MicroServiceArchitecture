using Auth.Service.ApiModels;
using Auth.Service.Infrastructure.Data;
using Auth.Service.Models;
using Auth.Service.Services.IService;
using ECommerce.Shared.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Service.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly AuthenticationOptions _authenticationOptions;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthStore _authStore;    

    public JwtTokenGenerator(AuthenticationOptions authenticationOptions,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IAuthStore authStore)
    {
        _authenticationOptions = authenticationOptions;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _authStore = authStore;        
    }
    private async Task<JwtSecurityToken> GenerateToken(ApplicationUser applicationUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_authenticationOptions.JwtOptions.SecurityKey);

        var userClaims = await _userManager.GetClaimsAsync(applicationUser);
        var roles = await _userManager.GetRolesAsync(applicationUser);
        var roleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new Claim(JwtRegisteredClaimNames.Name , applicationUser.UserName),
            new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
            new Claim("uid", applicationUser.Id),
        }
        .Union(roleClaims)
        .Union(userClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationOptions.JwtOptions.SecurityKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
                            issuer: _authenticationOptions.JwtOptions.Issuer,
                            audience: _authenticationOptions.JwtOptions.Audience,
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(_authenticationOptions.JwtOptions.DurationInMinutes),
                            signingCredentials: signingCredentials);
        //var tokenDescriptor = new SecurityTokenDescriptor
        //{
        //    Audience = _authenticationOptions.JwtOptions.Audience,
        //    Issuer = _authenticationOptions.JwtOptions.Issuer,
        //    Subject = new ClaimsIdentity(claimList),
        //    Expires = DateTime.UtcNow.AddDays(7),
        //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //};
        //var token = tokenHandler.CreateToken(tokenDescriptor);
        // return tokenHandler.WriteToken(token);
        return jwtSecurityToken;
    }

    public async Task<TokenResponseDto> GetTokenAsync(TokenRequestDto model)
    {
        var authenticationModel = new TokenResponseDto();
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"No Accounts Registered with {model.Email}.";
            return authenticationModel;
        }

        authenticationModel.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = await GenerateToken(user);
        authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        authenticationModel.Email = user.Email;
        authenticationModel.UserName = user.UserName;
        var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        authenticationModel.Roles = rolesList.ToList();

        if (user.RefreshTokens.Any(a => a.IsActive))
        {
            var activeRefreshToken = user.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
            authenticationModel.RefreshToken = activeRefreshToken.Token;
            authenticationModel.RefreshTokenExpiration = activeRefreshToken.Expires;
        }
        else
        {
            var refreshToken = CreateRefreshToken();
            authenticationModel.RefreshToken = refreshToken.Token;
            authenticationModel.RefreshTokenExpiration = refreshToken.Expires;
            user.RefreshTokens.Add(refreshToken);
            await _authStore.Update(user);
        }

        SetRefreshTokenInCookie(authenticationModel.RefreshToken);
        return authenticationModel;

        //authenticationModel.IsAuthenticated = false;
        //authenticationModel.Message = $"Incorrect Credentials for user {user.Email}.";
        //return authenticationModel;
    }
    /// <summary>
    /// Refresh 토큰 생성
    /// </summary>
    /// <returns></returns>
    private RefreshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            Expires = DateTime.UtcNow.AddDays(10),
            Created = DateTime.UtcNow
        };
    }
    private void SetRefreshTokenInCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(10),
        };
        _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

}
