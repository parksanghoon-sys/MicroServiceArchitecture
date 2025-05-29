using Auth.Service.Models;
using Auth.Service.Services.IService;
using ECommerce.Shared.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Service.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{    
    private readonly AuthenticationOptions _authenticationOptions;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtTokenGenerator(AuthenticationOptions authenticationOptions, UserManager<ApplicationUser> userManager)
    {        
        _authenticationOptions = authenticationOptions;
        _userManager = userManager;
    }
    public async Task<JwtSecurityToken> GenerateToken(ApplicationUser applicationUser)
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
}
