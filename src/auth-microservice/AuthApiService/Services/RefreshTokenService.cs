using AuthApiService.Interfaces;
using AuthApiService.Models;
using AuthApiService.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace AuthApiService.Services;

/// <summary>
/// 리프레시 토큰 서비스 구현 (인메모리)
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILoggingService _loggingService;
    private static readonly ConcurrentDictionary<string, RefreshToken> _refreshTokens = new();
    private static readonly ConcurrentDictionary<int, List<string>> _userTokens = new();
    private readonly Timer _cleanupTimer;
    private int _nextTokenId = 1;

    public RefreshTokenService(IOptions<JwtSettings> jwtSettings, ILoggingService loggingService)
    {
        _jwtSettings = jwtSettings.Value;
        _loggingService = loggingService;

        // 만료된 토큰 정리를 위한 타이머 (1시간마다 실행)
        _cleanupTimer = new Timer(PerformCleanup, null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
    }

    /// <summary>
    /// 리프레시 토큰 저장
    /// </summary>
    public async Task<RefreshToken> SaveRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        // 새 토큰 ID 할당
        var newTokenId = Interlocked.Increment(ref _nextTokenId);
        var newToken = refreshToken with { Id = newTokenId };

        // 토큰 저장
        _refreshTokens[newToken.Token] = newToken;

        // 사용자별 토큰 목록 업데이트
        _userTokens.AddOrUpdate(
            newToken.UserId,
            new List<string> { newToken.Token },
            (userId, existingTokens) =>
            {
                lock (existingTokens)
                {
                    existingTokens.Add(newToken.Token);
                    return existingTokens;
                }
            });

        await _loggingService.LogInformationAsync(
            "새 리프레시 토큰이 저장되었습니다",
            "RefreshTokenService",
            newToken.UserId,
            new Dictionary<string, object>
            {
                ["TokenId"] = newToken.Id,
                ["ExpiresAt"] = newToken.ExpiresAt,
                ["UserId"] = newToken.UserId
            });

        return newToken;
    }

    /// <summary>
    /// 리프레시 토큰 조회
    /// </summary>
    public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        _refreshTokens.TryGetValue(token, out var refreshToken);

        if (refreshToken != null)
        {
            await _loggingService.LogDebugAsync(
                "리프레시 토큰 조회 성공",
                "RefreshTokenService",
                refreshToken.UserId,
                new Dictionary<string, object>
                {
                    ["TokenId"] = refreshToken.Id,
                    ["IsValid"] = refreshToken.IsValid,
                    ["ExpiresAt"] = refreshToken.ExpiresAt,
                    ["IsRevoked"] = refreshToken.IsRevoked
                });
        }
        else
        {
            await _loggingService.LogWarningAsync(
                "존재하지 않는 리프레시 토큰 조회 시도",
                "RefreshTokenService",
                properties: new Dictionary<string, object>
                {
                    ["TokenPrefix"] = token.Length > 10 ? token[..10] + "..." : token
                });
        }

        return refreshToken;
    }

    /// <summary>
    /// 리프레시 토큰 무효화
    /// </summary>
    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        if (_refreshTokens.TryGetValue(token, out var refreshToken))
        {
            var revokedToken = refreshToken with { IsRevoked = true };
            _refreshTokens[token] = revokedToken;

            await _loggingService.LogInformationAsync(
                "리프레시 토큰이 무효화되었습니다",
                "RefreshTokenService",
                refreshToken.UserId,
                new Dictionary<string, object>
                {
                    ["TokenId"] = refreshToken.Id,
                    ["UserId"] = refreshToken.UserId,
                    ["RevokedAt"] = DateTime.UtcNow
                });
        }
        else
        {
            await _loggingService.LogWarningAsync(
                "존재하지 않는 리프레시 토큰 무효화 시도",
                "RefreshTokenService",
                properties: new Dictionary<string, object>
                {
                    ["TokenPrefix"] = token.Length > 10 ? token[..10] + "..." : token
                });
        }
    }

    /// <summary>
    /// 사용자의 모든 리프레시 토큰 무효화
    /// </summary>
    public async Task RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        if (_userTokens.TryGetValue(userId, out var userTokens))
        {
            var revokedCount = 0;
            
            lock (userTokens)
            {
                foreach (var tokenString in userTokens.ToList())
                {
                    if (_refreshTokens.TryGetValue(tokenString, out var token) && !token.IsRevoked)
                    {
                        var revokedToken = token with { IsRevoked = true };
                        _refreshTokens[tokenString] = revokedToken;
                        revokedCount++;
                    }
                }
            }

            await _loggingService.LogInformationAsync(
                $"사용자의 모든 리프레시 토큰이 무효화되었습니다 ({revokedCount}개)",
                "RefreshTokenService",
                userId,
                new Dictionary<string, object>
                {
                    ["UserId"] = userId,
                    ["RevokedTokenCount"] = revokedCount,
                    ["TotalTokenCount"] = userTokens.Count,
                    ["RevokedAt"] = DateTime.UtcNow
                });
        }
        else
        {
            await _loggingService.LogInformationAsync(
                "무효화할 리프레시 토큰이 없습니다",
                "RefreshTokenService",
                userId,
                new Dictionary<string, object>
                {
                    ["UserId"] = userId
                });
        }
    }

    /// <summary>
    /// 만료된 토큰 정리
    /// </summary>
    public async Task<int> CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        var expiredTokens = _refreshTokens.Values
            .Where(token => !token.IsValid)
            .ToList();

        var cleanedCount = 0;

        foreach (var expiredToken in expiredTokens)
        {
            if (_refreshTokens.TryRemove(expiredToken.Token, out _))
            {
                cleanedCount++;

                // 사용자별 토큰 목록에서도 제거
                if (_userTokens.TryGetValue(expiredToken.UserId, out var userTokens))
                {
                    lock (userTokens)
                    {
                        userTokens.Remove(expiredToken.Token);
                        
                        // 사용자의 토큰이 모두 정리되었으면 사용자 엔트리도 제거
                        if (!userTokens.Any())
                        {
                            _userTokens.TryRemove(expiredToken.UserId, out _);
                        }
                    }
                }
            }
        }

        if (cleanedCount > 0)
        {
            await _loggingService.LogInformationAsync(
                $"만료된 리프레시 토큰 정리 완료: {cleanedCount}개",
                "RefreshTokenService",
                properties: new Dictionary<string, object>
                {
                    ["CleanedTokenCount"] = cleanedCount,
                    ["TotalExpiredTokens"] = expiredTokens.Count,
                    ["CleanupTime"] = DateTime.UtcNow
                });
        }

        return cleanedCount;
    }

    /// <summary>
    /// 사용자의 활성 토큰 수 조회
    /// </summary>
    public async Task<int> GetActiveTokenCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        if (!_userTokens.TryGetValue(userId, out var userTokens))
            return 0;

        lock (userTokens)
        {
            var activeCount = userTokens.Count(tokenString =>
                _refreshTokens.TryGetValue(tokenString, out var token) && token.IsValid);

            return activeCount;
        }
    }

    /// <summary>
    /// 전체 토큰 통계 조회
    /// </summary>
    public async Task<Dictionary<string, object>> GetTokenStatisticsAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        var allTokens = _refreshTokens.Values.ToList();
        var activeTokens = allTokens.Where(t => t.IsValid).ToList();
        var expiredTokens = allTokens.Where(t => !t.IsValid).ToList();

        return new Dictionary<string, object>
        {
            ["TotalTokens"] = allTokens.Count,
            ["ActiveTokens"] = activeTokens.Count,
            ["ExpiredTokens"] = expiredTokens.Count,
            ["RevokedTokens"] = allTokens.Count(t => t.IsRevoked),
            ["UniqueUsers"] = _userTokens.Keys.Count,
            ["AverageTokensPerUser"] = _userTokens.Keys.Count > 0 
                ? (double)allTokens.Count / _userTokens.Keys.Count 
                : 0,
            ["OldestToken"] = allTokens.Any() 
                ? allTokens.Min(t => t.CreatedAt) 
                : (DateTime?)null,
            ["NewestToken"] = allTokens.Any() 
                ? allTokens.Max(t => t.CreatedAt) 
                : (DateTime?)null
        };
    }

    /// <summary>
    /// 정기적인 토큰 정리 수행
    /// </summary>
    private void PerformCleanup(object? state)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var cleanedCount = await CleanupExpiredTokensAsync();
                
                if (cleanedCount > 0)
                {
                    await _loggingService.LogInformationAsync(
                        $"정기 토큰 정리 완료: {cleanedCount}개 정리됨",
                        "RefreshTokenService",
                        properties: new Dictionary<string, object>
                        {
                            ["CleanupType"] = "Scheduled",
                            ["CleanedCount"] = cleanedCount
                        });
                }
            }
            catch (Exception ex)
            {
                await _loggingService.LogErrorAsync(
                    "정기 토큰 정리 중 오류 발생",
                    ex,
                    "RefreshTokenService");
            }
        });
    }

    /// <summary>
    /// 리소스 정리
    /// </summary>
    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        
        // 마지막 정리 수행
        _ = Task.Run(async () =>
        {
            try
            {
                await CleanupExpiredTokensAsync();
            }
            catch
            {
                // 정리 중 오류는 무시
            }
        });
    }
}
