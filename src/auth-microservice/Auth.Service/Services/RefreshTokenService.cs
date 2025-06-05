using Auth.Service.Infrastructure.Data.EntityFramework;
using Auth.Service.Models;
using Auth.Service.Services.IService;
using ECommerce.Shared.Authentication;
using System.Security.Cryptography;

namespace Auth.Service.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AuthContext _context;
    private readonly AuthenticationOptions _authenticationOptions;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(
      AuthContext context,
      AuthenticationOptions authenticationOptions,
      ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _authenticationOptions = authenticationOptions;
        _logger = logger;        
        _logger = logger;
        _context = context;
    }
    public Task<RefreshToken> GenerateRefreshTokenAsync(string userId, string ipAddress)
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[64];
        rng.GetBytes(randomBytes);

        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(_authenticationOptions.),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        _context.Users.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Refresh token generated for user {UserId} from IP {IpAddress}",
            userId, ipAddress);

        return refreshToken;
    }

    public Task<RefreshToken> GetRefreshTokenAsync(string tokenValue)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTokenActiveAsync(string tokenValue)
    {
        throw new NotImplementedException();
    }

    public Task RevokeAllUserTokensAsync(string userId, string reason = null)
    {
        throw new NotImplementedException();
    }
    public Task CleanupExpiredTokensAsync()
    {
        throw new NotImplementedException();
    }
    public Task RevokeRefreshTokenAsync(string tokenValue, string ipAddress, string reason = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateRefreshTokenAsync(string tokenValue)
    {
        throw new NotImplementedException();
    }
}
