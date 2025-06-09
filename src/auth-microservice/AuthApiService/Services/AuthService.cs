using AuthApiService.Interfaces;
using AuthApiService.Models;
using AuthApiService.Configuration;
using Microsoft.Extensions.Options;

namespace AuthApiService.Services;

/// <summary>
/// 인증 서비스 구현
/// </summary>
public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILoggingService _loggingService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        ITokenService tokenService,
        IUserService userService,
        IRefreshTokenService refreshTokenService,
        ILoggingService loggingService,
        IOptions<JwtSettings> jwtSettings)
    {
        _tokenService = tokenService;
        _userService = userService;
        _refreshTokenService = refreshTokenService;
        _loggingService = loggingService;
        _jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// 사용자 로그인
    /// </summary>
    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            await _loggingService.LogInformationAsync(
                $"로그인 시도: {request.Username}",
                "Authentication",
                properties: new Dictionary<string, object>
                {
                    ["Action"] = "Login",
                    ["Username"] = request.Username
                });

            // 입력 값 검증
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                await _loggingService.LogWarningAsync(
                    "로그인 실패: 사용자명 또는 비밀번호가 비어있음",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Reason"] = "EmptyCredentials",
                        ["Username"] = request.Username ?? "null"
                    });

                return AuthResult.Failure("사용자명과 비밀번호를 입력해주세요.");
            }

            // 사용자 조회 (사용자명 또는 이메일로)
            var user = await GetUserByUsernameOrEmailAsync(request.Username, cancellationToken);

            if (user == null)
            {
                await _loggingService.LogWarningAsync(
                    $"로그인 실패: 존재하지 않는 사용자 - {request.Username}",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Reason"] = "UserNotFound",
                        ["Username"] = request.Username
                    });

                return AuthResult.Failure("잘못된 사용자명 또는 비밀번호입니다.");
            }

            // 계정 활성화 상태 확인
            if (!user.IsActive)
            {
                await _loggingService.LogWarningAsync(
                    $"로그인 실패: 비활성화된 계정 - {user.Username}",
                    "Authentication",
                    user.UserId,
                    new Dictionary<string, object>
                    {
                        ["Reason"] = "AccountDeactivated",
                        ["Username"] = user.Username
                    });

                return AuthResult.Failure("계정이 비활성화되어 있습니다. 관리자에게 문의하세요.");
            }

            // 비밀번호 검증
            if (!_userService.VerifyPassword(request.Password, user.PasswordHash))
            {
                await _loggingService.LogWarningAsync(
                    $"로그인 실패: 잘못된 비밀번호 - {user.Username}",
                    "Authentication",
                    user.UserId,
                    new Dictionary<string, object>
                    {
                        ["Reason"] = "InvalidPassword",
                        ["Username"] = user.Username
                    });

                return AuthResult.Failure("잘못된 사용자명 또는 비밀번호입니다.");
            }

            await _loggingService.LogInformationAsync(
                $"로그인 성공: {user.Username}",
                "Authentication",
                user.UserId,
                new Dictionary<string, object>
                {
                    ["Action"] = "LoginSuccess",
                    ["Username"] = user.Username,
                    ["UserId"] = user.Id
                });

            return AuthResult.Success(user);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                $"로그인 처리 중 오류 발생: {request.Username}",
                ex,
                "Authentication",
                properties: new Dictionary<string, object>
                {
                    ["Username"] = request.Username,
                    ["ErrorType"] = ex.GetType().Name
                });

            return AuthResult.Failure("로그인 처리 중 오류가 발생했습니다. 잠시 후 다시 시도해주세요.");
        }
    }

    /// <summary>
    /// 사용자 등록
    /// </summary>
    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            await _loggingService.LogInformationAsync(
                $"사용자 등록 시도: {request.Username}",
                "Authentication",
                properties: new Dictionary<string, object>
                {
                    ["Action"] = "Register",
                    ["Username"] = request.Username,
                    ["Email"] = request.Email
                });

            // 입력 값 검증
            var validationResult = ValidateRegistrationRequest(request);
            if (!validationResult.IsValid)
            {
                await _loggingService.LogWarningAsync(
                    $"사용자 등록 실패: 유효성 검증 오류 - {request.Username}",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Reason"] = "ValidationError",
                        ["Username"] = request.Username,
                        ["ValidationErrors"] = validationResult.Errors
                    });

                return AuthResult.Failure(string.Join(", ", validationResult.Errors));
            }

            // 사용자명 중복 확인
            var existingUserByUsername = await _userService.GetUserByUsernameAsync(request.Username, cancellationToken);
            if (existingUserByUsername != null)
            {
                await _loggingService.LogWarningAsync(
                    $"사용자 등록 실패: 중복된 사용자명 - {request.Username}",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Reason"] = "DuplicateUsername",
                        ["Username"] = request.Username
                    });

                return AuthResult.Failure("이미 사용 중인 사용자명입니다.");
            }

            // 이메일 중복 확인
            var existingUserByEmail = await _userService.GetUserByEmailAsync(request.Email, cancellationToken);
            if (existingUserByEmail != null)
            {
                await _loggingService.LogWarningAsync(
                    $"사용자 등록 실패: 중복된 이메일 - {request.Email}",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Reason"] = "DuplicateEmail",
                        ["Email"] = request.Email,
                        ["Username"] = request.Username
                    });

                return AuthResult.Failure("이미 사용 중인 이메일입니다.");
            }

            // 비밀번호 해시화
            var hashedPassword = _userService.HashPassword(request.Password);

            // 새 사용자 생성
            var newUser = ApplicationUser.Create(request.Username, request.Email, hashedPassword);
            var createdUser = await _userService.CreateUserAsync(newUser, cancellationToken);

            await _loggingService.LogInformationAsync(
                $"사용자 등록 성공: {createdUser.Username}",
                "Authentication",
                createdUser.UserId,
                new Dictionary<string, object>
                {
                    ["Action"] = "RegisterSuccess",
                    ["Username"] = createdUser.Username,
                    ["Email"] = createdUser.Email,
                    ["UserId"] = createdUser.Id
                });

            return AuthResult.Success(createdUser);
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                $"사용자 등록 처리 중 오류 발생: {request.Username}",
                ex,
                "Authentication",
                properties: new Dictionary<string, object>
                {
                    ["Username"] = request.Username,
                    ["Email"] = request.Email,
                    ["ErrorType"] = ex.GetType().Name
                });

            return AuthResult.Failure("사용자 등록 처리 중 오류가 발생했습니다. 잠시 후 다시 시도해주세요.");
        }
    }

    /// <summary>
    /// 토큰 생성
    /// </summary>
    public async Task<TokenResponse> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        try
        {
            // 액세스 토큰 생성
            var accessToken = _tokenService.GenerateAccessToken(user);

            // 리프레시 토큰 생성
            var refreshTokenValue = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            
            var refreshToken = RefreshToken.Create(user.UserId, refreshTokenValue, refreshTokenExpiry);
            await _refreshTokenService.SaveRefreshTokenAsync(refreshToken, cancellationToken);

            var tokenResponse = new TokenResponse(
                accessToken,
                refreshTokenValue,
                _jwtSettings.AccessTokenExpirationMinutes * 60
            );

            await _loggingService.LogInformationAsync(
                $"토큰이 생성되었습니다: {user.Username}",
                "Authentication",
                user.UserId,
                new Dictionary<string, object>
                {
                    ["Action"] = "TokenGenerated",
                    ["Username"] = user.Username,
                    ["AccessTokenExpiration"] = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                    ["RefreshTokenExpiration"] = refreshTokenExpiry
                });

            return tokenResponse;
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                $"토큰 생성 중 오류 발생: {user.Username}",
                ex,
                "Authentication",
                user.UserId);

            throw new InvalidOperationException("토큰 생성 중 오류가 발생했습니다.", ex);
        }
    }

    /// <summary>
    /// 리프레시 토큰으로 새 액세스 토큰 발급
    /// </summary>
    public async Task<TokenResponse?> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                await _loggingService.LogWarningAsync(
                    "토큰 갱신 실패: 빈 리프레시 토큰",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Reason"] = "EmptyRefreshToken"
                    });

                return null;
            }

            // 리프레시 토큰 조회 및 검증
            var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (refreshToken == null || !refreshToken.IsValid)
            {
                await _loggingService.LogWarningAsync(
                    "토큰 갱신 실패: 유효하지 않은 리프레시 토큰",
                    "Authentication",
                    refreshToken?.UserId,
                    new Dictionary<string, object>
                    {
                        ["Reason"] = refreshToken == null ? "TokenNotFound" : "TokenInvalid",
                        ["TokenExpired"] = refreshToken?.ExpiresAt < DateTime.UtcNow,
                        ["TokenRevoked"] = refreshToken?.IsRevoked ?? false
                    });

                return null;
            }

            // 사용자 정보 조회
            var user = await _userService.GetUserByIdAsync(refreshToken.UserId, cancellationToken);

            if (user == null || !user.IsActive)
            {
                await _loggingService.LogWarningAsync(
                    "토큰 갱신 실패: 사용자를 찾을 수 없거나 비활성화됨",
                    "Authentication",
                    refreshToken.UserId,
                    new Dictionary<string, object>
                    {
                        ["Reason"] = user == null ? "UserNotFound" : "UserInactive",
                        ["UserId"] = refreshToken.UserId
                    });

                // 무효한 사용자의 토큰 무효화
                await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
                return null;
            }

            // 기존 리프레시 토큰 무효화 (한 번 사용 후 폐기)
            await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

            // 새 토큰 생성
            var newTokenResponse = await GenerateTokenAsync(user, cancellationToken);

            await _loggingService.LogInformationAsync(
                $"토큰 갱신 성공: {user.Username}",
                "Authentication",
                1,
                new Dictionary<string, object>
                {
                    ["Action"] = "TokenRefreshed",
                    ["Username"] = user.Username,
                    ["OldTokenRevoked"] = true,
                    ["NewTokenGenerated"] = true
                });

            return newTokenResponse;
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                "토큰 갱신 처리 중 오류 발생",
                ex,
                "Authentication",
                properties: new Dictionary<string, object>
                {
                    ["ErrorType"] = ex.GetType().Name
                });

            return null;
        }
    }

    /// <summary>
    /// 로그아웃 (토큰 무효화)
    /// </summary>
    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                await _loggingService.LogWarningAsync(
                    "로그아웃 요청: 빈 리프레시 토큰",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Reason"] = "EmptyRefreshToken"
                    });
                return;
            }

            var token = await _refreshTokenService.GetRefreshTokenAsync(refreshToken, cancellationToken);

            if (token != null)
            {
                await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

                await _loggingService.LogInformationAsync(
                    "로그아웃 성공",
                    "Authentication",
                    token.UserId,
                    new Dictionary<string, object>
                    {
                        ["Action"] = "Logout",
                        ["UserId"] = token.UserId,
                        ["TokenRevoked"] = true
                    });
            }
            else
            {
                await _loggingService.LogWarningAsync(
                    "로그아웃 실패: 토큰을 찾을 수 없음",
                    "Authentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Reason"] = "TokenNotFound"
                    });
            }
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                "로그아웃 처리 중 오류 발생",
                ex,
                "Authentication");
        }
    }

    /// <summary>
    /// 사용자의 모든 토큰 무효화
    /// </summary>
    public async Task RevokeAllTokensAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _refreshTokenService.RevokeAllUserTokensAsync(userId, cancellationToken);

            await _loggingService.LogInformationAsync(
                "사용자의 모든 토큰이 무효화되었습니다",
                "Authentication",
                userId,
                new Dictionary<string, object>
                {
                    ["Action"] = "RevokeAllTokens",
                    ["UserId"] = userId
                });
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                $"사용자 토큰 무효화 중 오류 발생: UserId={userId}",
                ex,
                "Authentication",
                userId);
        }
    }

    /// <summary>
    /// 사용자명 또는 이메일로 사용자 조회
    /// </summary>
    private async Task<ApplicationUser?> GetUserByUsernameOrEmailAsync(string usernameOrEmail, CancellationToken cancellationToken)
    {
        // 먼저 사용자명으로 시도
        var user = await _userService.GetUserByUsernameAsync(usernameOrEmail, cancellationToken);

        // 사용자명으로 찾지 못했고 이메일 형식이면 이메일로 시도
        if (user == null && IsValidEmail(usernameOrEmail))
        {
            user = await _userService.GetUserByEmailAsync(usernameOrEmail, cancellationToken);
        }

        return user;
    }

    /// <summary>
    /// 이메일 형식 검증
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 회원가입 요청 유효성 검증
    /// </summary>
    private static (bool IsValid, List<string> Errors) ValidateRegistrationRequest(RegisterRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            errors.Add("사용자명을 입력해주세요.");
        }
        else if (request.Username.Length < 3 || request.Username.Length > 50)
        {
            errors.Add("사용자명은 3자 이상 50자 이하여야 합니다.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("이메일을 입력해주세요.");
        }
        else if (!IsValidEmail(request.Email))
        {
            errors.Add("올바른 이메일 형식을 입력해주세요.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add("비밀번호를 입력해주세요.");
        }

        return (errors.Count == 0, errors);
    }
}
