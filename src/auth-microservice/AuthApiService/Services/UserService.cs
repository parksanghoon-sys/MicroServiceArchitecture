using AuthApiService.Interfaces;
using AuthApiService.Models;
using AuthApiService.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using BCrypt.Net;

namespace AuthApiService.Services;

/// <summary>
/// 사용자 서비스 구현 (인메모리)
/// </summary>
public class UserService : IUserService
{
    private readonly SecuritySettings _securitySettings;
    private readonly ILoggingService _loggingService;
    private static readonly ConcurrentDictionary<int, ApplicationUser> _users = new();
    private static readonly ConcurrentDictionary<string, ApplicationUser> _usersByUsername = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, ApplicationUser> _usersByEmail = new(StringComparer.OrdinalIgnoreCase);
    private int _nextUserId = 1;

    public UserService(IOptions<SecuritySettings> securitySettings, ILoggingService loggingService)
    {
        _securitySettings = securitySettings.Value;
        _loggingService = loggingService;

        // 기본 테스트 사용자 추가
        InitializeTestUsers();
    }

    /// <summary>
    /// 사용자명으로 사용자 조회
    /// </summary>
    public async Task<ApplicationUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        _usersByUsername.TryGetValue(username.Trim(), out var user);

        if (user != null)
        {
            await _loggingService.LogDebugAsync(
                $"사용자 조회 성공: {username}",
                "UserService",
                user.UserId,
                new Dictionary<string, object>
                {
                    ["SearchType"] = "Username",
                    ["Found"] = true
                });
        }

        return user;
    }

    /// <summary>
    /// 이메일로 사용자 조회
    /// </summary>
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        _usersByEmail.TryGetValue(email.Trim(), out var user);

        if (user != null)
        {
            await _loggingService.LogDebugAsync(
                $"사용자 조회 성공: {email}",
                "UserService",
                user.UserId,
                new Dictionary<string, object>
                {
                    ["SearchType"] = "Email",
                    ["Found"] = true
                });
        }

        return user;
    }

    /// <summary>
    /// 사용자 ID로 사용자 조회
    /// </summary>
    public async Task<ApplicationUser?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        _users.TryGetValue(userId, out var user);
        
        if (user != null)
        {
            await _loggingService.LogDebugAsync(
                $"사용자 ID로 조회 성공: {userId}",
                "UserService",
                userId,
                new Dictionary<string, object>
                {
                    ["SearchType"] = "UserId",
                    ["Found"] = true
                });
        }

        return user;
    }

    /// <summary>
    /// 새 사용자 생성
    /// </summary>
    public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션

        // 사용자명과 이메일 중복 확인
        if (_usersByUsername.ContainsKey(user.Username))
        {
            await _loggingService.LogWarningAsync(
                $"사용자 생성 실패 - 중복된 사용자명: {user.Username}",
                "UserService",
                properties: new Dictionary<string, object>
                {
                    ["Reason"] = "DuplicateUsername",
                    ["Username"] = user.Username
                });
            
            throw new InvalidOperationException("이미 존재하는 사용자명입니다.");
        }

        if (_usersByEmail.ContainsKey(user.Email))
        {
            await _loggingService.LogWarningAsync(
                $"사용자 생성 실패 - 중복된 이메일: {user.Email}",
                "UserService",
                properties: new Dictionary<string, object>
                {
                    ["Reason"] = "DuplicateEmail",
                    ["Email"] = user.Email
                });
            
            throw new InvalidOperationException("이미 존재하는 이메일입니다.");
        }

        // 새 사용자 ID 할당
        var newUserId = Interlocked.Increment(ref _nextUserId);        
        // Replace the following line:        
        // With this updated code:
        var newUser = new ApplicationUser
        {
            UserId = newUserId,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            LastLoginIp = user.LastLoginIp,
            RegistrationIp = user.RegistrationIp,
            IsActive = user.IsActive,
            RefreshTokens = user.RefreshTokens
        };
        // 사용자 저장
        _users[newUserId] = user;
        _usersByUsername[newUser.Username] = newUser;
        _usersByEmail[newUser.Email] = newUser;

        await _loggingService.LogInformationAsync(
            $"새 사용자가 생성되었습니다: {newUser.Username}",
            "UserService",
            newUserId,
            new Dictionary<string, object>
            {
                ["Username"] = newUser.Username,
                ["Email"] = newUser.Email,
                ["CreatedAt"] = newUser.CreatedAt
            });

        return newUser;
    }

    /// <summary>
    /// 비밀번호 검증
    /// </summary>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () =>
            {
                await _loggingService.LogErrorAsync(
                    "비밀번호 검증 중 오류 발생",
                    ex,
                    "UserService",
                    properties: new Dictionary<string, object>
                    {
                        ["ErrorType"] = "PasswordVerificationError"
                    });
            });
            return false;
        }
    }

    /// <summary>
    /// 비밀번호 해시 생성
    /// </summary>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("비밀번호는 null이거나 빈 문자열일 수 없습니다.", nameof(password));

        // 비밀번호 강도 검증
        ValidatePasswordStrength(password);

        try
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () =>
            {
                await _loggingService.LogErrorAsync(
                    "비밀번호 해시 생성 중 오류 발생",
                    ex,
                    "UserService",
                    properties: new Dictionary<string, object>
                    {
                        ["ErrorType"] = "PasswordHashingError"
                    });
            });
            throw new InvalidOperationException("비밀번호 해시 생성 중 오류가 발생했습니다.", ex);
        }
    }

    /// <summary>
    /// 비밀번호 강도 검증
    /// </summary>
    private void ValidatePasswordStrength(string password)
    {
        var errors = new List<string>();

        if (password.Length < _securitySettings.MinPasswordLength)
        {
            errors.Add($"비밀번호는 최소 {_securitySettings.MinPasswordLength}자 이상이어야 합니다.");
        }

        if (_securitySettings.RequireUppercase && !password.Any(char.IsUpper))
        {
            errors.Add("비밀번호에는 대문자가 포함되어야 합니다.");
        }

        if (_securitySettings.RequireLowercase && !password.Any(char.IsLower))
        {
            errors.Add("비밀번호에는 소문자가 포함되어야 합니다.");
        }

        if (_securitySettings.RequireDigit && !password.Any(char.IsDigit))
        {
            errors.Add("비밀번호에는 숫자가 포함되어야 합니다.");
        }

        if (_securitySettings.RequireSpecialCharacter && !password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            errors.Add("비밀번호에는 특수문자가 포함되어야 합니다.");
        }

        if (errors.Any())
        {
            var errorMessage = string.Join(" ", errors);
            
            _ = Task.Run(async () =>
            {
                await _loggingService.LogWarningAsync(
                    $"비밀번호 강도 검증 실패: {errorMessage}",
                    "UserService",
                    properties: new Dictionary<string, object>
                    {
                        ["ValidationErrors"] = errors,
                        ["PasswordLength"] = password.Length
                    });
            });

            throw new ArgumentException(errorMessage, nameof(password));
        }
    }

    /// <summary>
    /// 테스트 사용자 초기화
    /// </summary>
    private void InitializeTestUsers()
    {
        try
        {
            var testUsers = new[]
            {
                ApplicationUser.Create("admin", "admin@example.com", HashPassword("Admin123!")),
                ApplicationUser.Create("testuser", "test@example.com", HashPassword("Test123!")),
                ApplicationUser.Create("demo", "demo@example.com", HashPassword("Demo123!"))
            };

            foreach (var user in testUsers)
            {
                var newUserId = Interlocked.Increment(ref _nextUserId);
                var newUser = new ApplicationUser { UserId = newUserId, Username = user.Username, Email = user.Email, PasswordHash = user.PasswordHash, CreatedAt = user.CreatedAt, LastLoginAt = user.LastLoginAt, LastLoginIp = user.LastLoginIp, RegistrationIp = user.RegistrationIp, IsActive = user.IsActive, RefreshTokens = user.RefreshTokens };

                _users[newUserId] = newUser;
                _usersByUsername[newUser.Username] = newUser;
                _usersByEmail[newUser.Email] = newUser;
            }

            _ = Task.Run(async () =>
            {
                await _loggingService.LogInformationAsync(
                    $"{testUsers.Length}개의 테스트 사용자가 초기화되었습니다",
                    "UserService",
                    properties: new Dictionary<string, object>
                    {
                        ["TestUserCount"] = testUsers.Length,
                        ["Usernames"] = testUsers.Select(u => u.Username).ToArray()
                    });
            });
        }
        catch (Exception ex)
        {
            _ = Task.Run(async () =>
            {
                await _loggingService.LogErrorAsync(
                    "테스트 사용자 초기화 중 오류 발생",
                    ex,
                    "UserService");
            });
        }
    }
}
