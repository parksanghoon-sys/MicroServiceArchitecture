using AuthApiService.Models;

namespace AuthApiService.Interfaces;

/// <summary>
/// 인증 서비스 인터페이스
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 사용자 로그인
    /// </summary>
    /// <param name="request">로그인 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>인증 결과</returns>
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 사용자 등록
    /// </summary>
    /// <param name="request">등록 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>등록 결과</returns>
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 토큰 생성
    /// </summary>
    /// <param name="user">사용자 정보</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>토큰 응답</returns>
    Task<TokenResponse> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// 리프레시 토큰으로 새 액세스 토큰 발급
    /// </summary>
    /// <param name="request">리프레시 토큰 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>새 토큰 응답</returns>
    Task<TokenResponse?> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 로그아웃 (토큰 무효화)
    /// </summary>
    /// <param name="refreshToken">리프레시 토큰</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// 사용자의 모든 토큰 무효화
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task RevokeAllTokensAsync(int userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// 토큰 서비스 인터페이스
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// JWT 액세스 토큰 생성
    /// </summary>
    /// <param name="user">사용자 정보</param>
    /// <returns>JWT 토큰</returns>
    string GenerateAccessToken(ApplicationUser user);

    /// <summary>
    /// 리프레시 토큰 생성
    /// </summary>
    /// <returns>리프레시 토큰</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// JWT 토큰에서 사용자 ID 추출
    /// </summary>
    /// <param name="token">JWT 토큰</param>
    /// <returns>사용자 ID</returns>
    int? GetUserIdFromToken(string token);

    /// <summary>
    /// JWT 토큰 유효성 검증
    /// </summary>
    /// <param name="token">JWT 토큰</param>
    /// <returns>유효성 여부</returns>
    bool ValidateToken(string token);

    /// <summary>
    /// 토큰 만료 시간 가져오기
    /// </summary>
    /// <param name="token">JWT 토큰</param>
    /// <returns>만료 시간</returns>
    DateTime? GetTokenExpiration(string token);
}

/// <summary>
/// 사용자 서비스 인터페이스
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 사용자명으로 사용자 조회
    /// </summary>
    /// <param name="username">사용자명</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>사용자 정보</returns>
    Task<ApplicationUser?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// 이메일로 사용자 조회
    /// </summary>
    /// <param name="email">이메일</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>사용자 정보</returns>
    Task<ApplicationUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// 사용자 ID로 사용자 조회
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>사용자 정보</returns>
    Task<ApplicationUser?> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 새 사용자 생성
    /// </summary>
    /// <param name="user">사용자 정보</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>생성된 사용자</returns>
    Task<ApplicationUser> CreateUserAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// 비밀번호 검증
    /// </summary>
    /// <param name="password">평문 비밀번호</param>
    /// <param name="hashedPassword">해시된 비밀번호</param>
    /// <returns>검증 결과</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// 비밀번호 해시 생성
    /// </summary>
    /// <param name="password">평문 비밀번호</param>
    /// <returns>해시된 비밀번호</returns>
    string HashPassword(string password);
}

/// <summary>
/// 리프레시 토큰 서비스 인터페이스
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// 리프레시 토큰 저장
    /// </summary>
    /// <param name="refreshToken">리프레시 토큰 정보</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>저장된 토큰</returns>
    Task<RefreshToken> SaveRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// 리프레시 토큰 조회
    /// </summary>
    /// <param name="token">토큰 값</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>리프레시 토큰 정보</returns>
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 리프레시 토큰 무효화
    /// </summary>
    /// <param name="token">토큰 값</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 사용자의 모든 리프레시 토큰 무효화
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 만료된 토큰 정리
    /// </summary>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>정리된 토큰 수</returns>
    Task<int> CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
}
