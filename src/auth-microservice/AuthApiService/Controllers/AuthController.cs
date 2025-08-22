using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuthApiService.Interfaces;
using AuthApiService.Models;
using System.Security.Claims;

namespace AuthApiService.Controllers;

/// <summary>
/// 인증 관련 API 컨트롤러
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILoggingService _loggingService;

    public AuthController(IAuthService authService, ILoggingService loggingService)
    {
        _authService = authService;
        _loggingService = loggingService;
    }

    /// <summary>
    /// 사용자 로그인
    /// </summary>
    /// <param name="request">로그인 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>로그인 결과</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                await _loggingService.LogWarningAsync(
                    "로그인 요청 유효성 검증 실패",
                    "API",
                    properties: new Dictionary<string, object>
                    {
                        ["Endpoint"] = "POST /api/auth/login",
                        ["ValidationErrors"] = errors,
                        ["ClientIP"] = GetClientIPAddress()
                    });

                return BadRequest(ApiResponse<object>.ErrorResponse("요청 데이터가 올바르지 않습니다.", errors));
            }

            var authResult = await _authService.LoginAsync(request, cancellationToken);

            if (!authResult.IsSuccess)
            {
                await _loggingService.LogAuthenticationAsync(
                    "Login",
                    request.Username,
                    false,
                    GetClientIPAddress(),
                    GetUserAgent(),
                    new Dictionary<string, object>
                    {
                        ["Error"] = authResult.ErrorMessage ?? "Unknown error"
                    });

                return Unauthorized(ApiResponse<object>.ErrorResponse(authResult.ErrorMessage ?? "로그인에 실패했습니다."));
            }

            var tokenResponse = await _authService.GenerateTokenAsync(authResult.User!, cancellationToken);

            await _loggingService.LogAuthenticationAsync(
                "Login",
                request.Username,
                true,
                GetClientIPAddress(),
                GetUserAgent(),
                new Dictionary<string, object>
                {
                    ["UserId"] = authResult.User!.Id,
                    ["TokenGenerated"] = true
                });

            return Ok(ApiResponse<TokenResponse>.SuccessResponse(tokenResponse, "로그인에 성공했습니다."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                "로그인 API 처리 중 오류 발생",
                ex,
                "API",
                properties: new Dictionary<string, object>
                {
                    ["Endpoint"] = "POST /api/auth/login",
                    ["Username"] = request?.Username ?? "null",
                    ["ClientIP"] = GetClientIPAddress()
                });

            return StatusCode(500, ApiResponse<object>.ErrorResponse("서버 내부 오류가 발생했습니다."));
        }
    }

    /// <summary>
    /// 사용자 등록
    /// </summary>
    /// <param name="request">등록 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>등록 결과</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> RegisterAsync(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                await _loggingService.LogWarningAsync(
                    "회원가입 요청 유효성 검증 실패",
                    "API",
                    properties: new Dictionary<string, object>
                    {
                        ["Endpoint"] = "POST /api/auth/register",
                        ["ValidationErrors"] = errors,
                        ["ClientIP"] = GetClientIPAddress()
                    });

                return BadRequest(ApiResponse<object>.ErrorResponse("요청 데이터가 올바르지 않습니다.", errors));
            }

            var authResult = await _authService.RegisterAsync(request, cancellationToken);

            if (!authResult.IsSuccess)
            {
                await _loggingService.LogWarningAsync(
                    $"회원가입 실패: {request.Username}",
                    "API",
                    properties: new Dictionary<string, object>
                    {
                        ["Endpoint"] = "POST /api/auth/register",
                        ["Username"] = request.Username,
                        ["Email"] = request.Email,
                        ["Error"] = authResult.ErrorMessage ?? "Unknown error",
                        ["ClientIP"] = GetClientIPAddress()
                    });

                var statusCode = authResult.ErrorMessage?.Contains("중복") == true ? 409 : 400;
                return StatusCode(statusCode, ApiResponse<object>.ErrorResponse(authResult.ErrorMessage ?? "회원가입에 실패했습니다."));
            }

            var tokenResponse = await _authService.GenerateTokenAsync(authResult.User!, cancellationToken);

            await _loggingService.LogInformationAsync(
                $"회원가입 성공: {authResult.User!.Username}",
                "API",
                authResult.User.UserId,
                new Dictionary<string, object>
                {
                    ["Endpoint"] = "POST /api/auth/register",
                    ["Username"] = authResult.User.Username,
                    ["Email"] = authResult.User.Email,
                    ["ClientIP"] = GetClientIPAddress()
                });

            return StatusCode(201, ApiResponse<TokenResponse>.SuccessResponse(tokenResponse, "회원가입에 성공했습니다."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                "회원가입 API 처리 중 오류 발생",
                ex,
                "API",
                properties: new Dictionary<string, object>
                {
                    ["Endpoint"] = "POST /api/auth/register",
                    ["Username"] = request?.Username ?? "null",
                    ["Email"] = request?.Email ?? "null",
                    ["ClientIP"] = GetClientIPAddress()
                });

            return StatusCode(500, ApiResponse<object>.ErrorResponse("서버 내부 오류가 발생했습니다."));
        }
    }

    /// <summary>
    /// 토큰 갱신
    /// </summary>
    /// <param name="request">토큰 갱신 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>새로운 토큰</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<TokenResponse>>> RefreshTokenAsync(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                await _loggingService.LogWarningAsync(
                    "토큰 갱신 요청 유효성 검증 실패",
                    "API",
                    properties: new Dictionary<string, object>
                    {
                        ["Endpoint"] = "POST /api/auth/refresh",
                        ["ClientIP"] = GetClientIPAddress(),
                        ["HasRefreshToken"] = !string.IsNullOrWhiteSpace(request?.RefreshToken)
                    });

                return BadRequest(ApiResponse<object>.ErrorResponse("유효한 리프레시 토큰을 제공해주세요."));
            }

            var tokenResponse = await _authService.RefreshTokenAsync(request, cancellationToken);

            if (tokenResponse == null)
            {
                await _loggingService.LogWarningAsync(
                    "토큰 갱신 실패: 유효하지 않은 리프레시 토큰",
                    "API",
                    properties: new Dictionary<string, object>
                    {
                        ["Endpoint"] = "POST /api/auth/refresh",
                        ["ClientIP"] = GetClientIPAddress()
                    });

                return Unauthorized(ApiResponse<object>.ErrorResponse("유효하지 않은 리프레시 토큰입니다."));
            }

            await _loggingService.LogInformationAsync(
                "토큰 갱신 성공",
                "API",
                properties: new Dictionary<string, object>
                {
                    ["Endpoint"] = "POST /api/auth/refresh",
                    ["ClientIP"] = GetClientIPAddress()
                });

            return Ok(ApiResponse<TokenResponse>.SuccessResponse(tokenResponse, "토큰이 갱신되었습니다."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                "토큰 갱신 API 처리 중 오류 발생",
                ex,
                "API",
                properties: new Dictionary<string, object>
                {
                    ["Endpoint"] = "POST /api/auth/refresh",
                    ["ClientIP"] = GetClientIPAddress()
                });

            return StatusCode(500, ApiResponse<object>.ErrorResponse("서버 내부 오류가 발생했습니다."));
        }
    }

    /// <summary>
    /// 로그아웃
    /// </summary>
    /// <param name="request">로그아웃 요청</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>로그아웃 결과</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> LogoutAsync(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                await _loggingService.LogWarningAsync(
                    "로그아웃 요청: 리프레시 토큰 누락",
                    "API",
                    userId,
                    new Dictionary<string, object>
                    {
                        ["Endpoint"] = "POST /api/auth/logout",
                        ["ClientIP"] = GetClientIPAddress()
                    });

                return BadRequest(ApiResponse<object>.ErrorResponse("리프레시 토큰을 제공해주세요."));
            }

            await _authService.LogoutAsync(request.RefreshToken, cancellationToken);

            await _loggingService.LogInformationAsync(
                "로그아웃 성공",
                "API",
                userId,
                new Dictionary<string, object>
                {
                    ["Endpoint"] = "POST /api/auth/logout",
                    ["ClientIP"] = GetClientIPAddress()
                });

            return Ok(ApiResponse<object>.SuccessResponse(null, "로그아웃되었습니다."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                "로그아웃 API 처리 중 오류 발생",
                ex,
                "API",
                GetCurrentUserId(),
                new Dictionary<string, object>
                {
                    ["Endpoint"] = "POST /api/auth/logout",
                    ["ClientIP"] = GetClientIPAddress()
                });

            return StatusCode(500, ApiResponse<object>.ErrorResponse("서버 내부 오류가 발생했습니다."));
        }
    }

    /// <summary>
    /// 모든 토큰 무효화
    /// </summary>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>무효화 결과</returns>
    [HttpPost("revoke-all")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> RevokeAllTokensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("인증이 필요합니다."));
            }

            await _authService.RevokeAllTokensAsync(userId.Value, cancellationToken);

            await _loggingService.LogInformationAsync(
                "모든 토큰 무효화 성공",
                "API",
                userId,
                new Dictionary<string, object>
                {
                    ["Endpoint"] = "POST /api/auth/revoke-all",
                    ["ClientIP"] = GetClientIPAddress()
                });

            return Ok(ApiResponse<object>.SuccessResponse(null, "모든 토큰이 무효화되었습니다."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                "모든 토큰 무효화 API 처리 중 오류 발생",
                ex,
                "API",
                GetCurrentUserId(),
                new Dictionary<string, object>
                {
                    ["Endpoint"] = "POST /api/auth/revoke-all",
                    ["ClientIP"] = GetClientIPAddress()
                });

            return StatusCode(500, ApiResponse<object>.ErrorResponse("서버 내부 오류가 발생했습니다."));
        }
    }

    /// <summary>
    /// 현재 사용자 정보 조회
    /// </summary>
    /// <returns>사용자 정보</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> GetCurrentUserAsync()
    {
        try
        {
            var userId = GetCurrentUserId();
            var username = GetCurrentUsername();

            if (!userId.HasValue || string.IsNullOrWhiteSpace(username))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("인증이 필요합니다."));
            }

            var userInfo = new
            {
                Id = userId.Value,
                Username = username,
                Email = GetCurrentUserEmail(),
                IsAuthenticated = true,
                LoginTime = DateTime.UtcNow
            };

            await _loggingService.LogInformationAsync(
                "사용자 정보 조회",
                "API",
                userId,
                new Dictionary<string, object>
                {
                    ["Endpoint"] = "GET /api/auth/me",
                    ["Username"] = username,
                    ["ClientIP"] = GetClientIPAddress()
                });

            return Ok(ApiResponse<object>.SuccessResponse(userInfo, "사용자 정보를 조회했습니다."));
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync(
                "사용자 정보 조회 API 처리 중 오류 발생",
                ex,
                "API",
                GetCurrentUserId(),
                new Dictionary<string, object>
                {
                    ["Endpoint"] = "GET /api/auth/me",
                    ["ClientIP"] = GetClientIPAddress()
                });

            return StatusCode(500, ApiResponse<object>.ErrorResponse("서버 내부 오류가 발생했습니다."));
        }
    }

    /// <summary>
    /// 현재 사용자 ID 가져오기
    /// </summary>
    /// <returns>사용자 ID</returns>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("user_id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }

    /// <summary>
    /// 현재 사용자명 가져오기
    /// </summary>
    /// <returns>사용자명</returns>
    private string? GetCurrentUsername()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name;
    }

    /// <summary>
    /// 현재 사용자 이메일 가져오기
    /// </summary>
    /// <returns>이메일</returns>
    private string? GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// 클라이언트 IP 주소 가져오기
    /// </summary>
    /// <returns>IP 주소</returns>
    private string GetClientIPAddress()
    {
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp))
        {
            return realIp;
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    /// <summary>
    /// User Agent 가져오기
    /// </summary>
    /// <returns>User Agent</returns>
    private string? GetUserAgent()
    {
        return Request.Headers["User-Agent"].FirstOrDefault();
    }
}
