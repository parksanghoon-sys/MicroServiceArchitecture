using Auth.Service.Models;
using Auth.Service.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Service.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationOptions _authenticationOptions;

        public JwtTokenService(UserManager<ApplicationUser> userManager,
            AuthenticationOptions authenticationOptions)
        {
            _userManager = userManager;
            _authenticationOptions = authenticationOptions;
        }

        public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_authenticationOptions.JwtOptions!.SecurityKey);
            var expiresAt = DateTime.UtcNow.AddMinutes(_authenticationOptions.JwtOptions.AccessTokenExpirationMinutes);

            // 사용자 역할 및 클레임 조회
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var tokenClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
            // 역할 추가
            foreach (var role in roles)
                tokenClaims.Add(new Claim(ClaimTypes.Role, role));

            // 사용자 정의 클레임 추가
            tokenClaims.AddRange(claims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                Expires = expiresAt,
                Issuer = _authenticationOptions.JwtOptions.Issuer,
                Audience = _authenticationOptions.JwtOptions.Audience,
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_authenticationOptions.JwtOptions!.SecurityKey);
            try
            {
                var principal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _authenticationOptions.JwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _authenticationOptions.JwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                // 토큰 만료
                return null;
            }
            catch (SecurityTokenException)
            {
                // 토큰 무효
                return null;
            }
            catch (Exception)
            {
                // 기타 오류
                return null;
            }
        }
        public DateTime GetTokenExpiry(string token)
        {
            if (string.IsNullOrEmpty(token))
                return DateTime.MinValue;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);

                return jsonToken.ValidTo;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public bool IsTokenExpired(string token)
        {
            if (string.IsNullOrEmpty(token))
                return true;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token);

                return jsonToken.ValidTo <= DateTime.UtcNow;
            }
            catch (Exception)
            {
                return true;                
            }
        }
    }

}
