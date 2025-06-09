using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AuthApiService.Interfaces;
using AuthApiService.Models;
using AuthApiService.Configuration;
using Microsoft.Extensions.Options;

namespace AuthApiService.Services;

/// <summary>
/// JWT 토큰 서비스 구현
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILoggingService _loggingService;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public TokenService(IOptions<JwtSettings> jwtSettings, ILoggingService loggingService)
    {
        _jwtSettings = jwtSettings.Value;
        _loggingService = loggingService;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    /// <summary>
    /// JWT 액세스 토큰 생성
    /// </summary>
    /// <param name="user">사용자 정보</param>
    /// <returns>JWT 토큰</returns>
    public string GenerateAccessToken(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64),
            new Claim("user_id", user.Id.ToString()),
            new Claim("is_active", user.IsActive.ToString().ToLower())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = credentials
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = _tokenHandler.WriteToken(token);

        _ = Task.Run(async () =>
        {
            await _loggingService.LogInformationAsync(
                "액세스 토큰이 생성되었습니다",
                "Authentication",
                user.UserId,
                new Dictionary<string, object>
                {
                    ["TokenType"] = "AccessToken",
                    ["ExpiresAt"] = tokenDescriptor.Expires,
                    ["Username"] = user.Username
                });
        });

        return tokenString;
    }

    /// <summary>
    /// 안전한 리프레시 토큰 생성
    /// </summary>
    /// <returns>리프레시 토큰</returns>
    public string GenerateRefreshToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[64];
        rng.GetBytes(randomBytes);
        
        var refreshToken = Convert.ToBase64String(randomBytes);

        _ = Task.Run(async () =>
        {
            await _loggingService.LogDebugAsync(
                "리프레시 토큰이 생성되었습니다",
                "Authentication",
                properties: new Dictionary<string, object>
                {
                    ["TokenType"] = "RefreshToken",
                    ["TokenLength"] = refreshToken.Length
                });
        });

        return refreshToken;
    }

    /// <summary>
    /// JWT 토큰에서 사용자 ID 추출
    /// </summary>
    /// <param name="token">JWT 토큰</param>
    /// <returns>사용자 ID</returns>
    public int? GetUserIdFromToken(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenValidationParameters = GetTokenValidationParameters();
            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            
            var userIdClaim = principal.FindFirst("user_id") ?? principal.FindFirst(ClaimTypes.NameIdentifier);
            
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) 
                ? userId 
                : null;
        }
        catch (SecurityTokenException ex)
        {
            _ = Task.Run(async () =>
            {
                await _loggingService.LogWarningAsync(
                    "토큰에서 사용자 ID 추출 실패",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Error"] = ex.Message,
                        ["TokenPrefix"] = token.Length > 10 ? token[..10] + "..." : token
                    });
            });
            return null;
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () =>
            {
                await _loggingService.LogErrorAsync(
                    "토큰 파싱 중 예상치 못한 오류 발생",
                    ex,
                    "Authentication");
            });
            return null;
        }
    }

    /// <summary>
    /// JWT 토큰 유효성 검증
    /// </summary>
    /// <param name="token">JWT 토큰</param>
    /// <returns>유효성 여부</returns>
    public bool ValidateToken(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var tokenValidationParameters = GetTokenValidationParameters();
            _tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            
            return true;
        }
        catch (SecurityTokenExpiredException)
        {
            _ = Task.Run(async () =>
            {
                await _loggingService.LogInformationAsync(
                    "만료된 토큰 검증 시도",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["TokenPrefix"] = token.Length > 10 ? token[..10] + "..." : token
                    });
            });
            return false;
        }
        catch (SecurityTokenException ex)
        {
            _ = Task.Run(async () =>
            {
                await _loggingService.LogWarningAsync(
                    "유효하지 않은 토큰 검증 시도",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Error"] = ex.Message,
                        ["TokenPrefix"] = token.Length > 10 ? token[..10] + "..." : token
                    });
            });
            return false;
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () =>
            {
                await _loggingService.LogErrorAsync(
                    "토큰 검증 중 예상치 못한 오류 발생",
                    ex,
                    "Authentication");
            });
            return false;
        }
    }

    /// <summary>
    /// 토큰 만료 시간 가져오기
    /// </summary>
    /// <param name="token">JWT 토큰</param>
    /// <returns>만료 시간</returns>
    public DateTime? GetTokenExpiration(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenValidationParameters = GetTokenValidationParameters();
            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            
            if (validatedToken is JwtSecurityToken jwtToken)
            {
                return jwtToken.ValidTo;
            }

            return null;
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () =>
            {
                await _loggingService.LogWarningAsync(
                    "토큰 만료 시간 추출 실패",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Error"] = ex.Message
                    });
            });
            return null;
        }
    }

    /// <summary>
    /// 토큰 검증 매개변수 생성
    /// </summary>
    /// <returns>토큰 검증 매개변수</returns>
    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5), // 클럭 스큐 허용
            RequireExpirationTime = true
        };
    }
}
