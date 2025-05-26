using Auth.Service.Models;
using Auth.Service.Services.IService;
using ECommerce.Shared.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Service.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{    
    private readonly AuthenticationOptions _authenticationOptions;

    public JwtTokenGenerator(AuthenticationOptions authenticationOptions)
    {        
        _authenticationOptions = authenticationOptions;
    }
    public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(_authenticationOptions.JwtOptions.SecurityKey);

        var claimList = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new Claim(JwtRegisteredClaimNames.Name , applicationUser.UserName),
        };
        claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _authenticationOptions.JwtOptions.Audience,
            Issuer = _authenticationOptions.JwtOptions.Issuer,
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
