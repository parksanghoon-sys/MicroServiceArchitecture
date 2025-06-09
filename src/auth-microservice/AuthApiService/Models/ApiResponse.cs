namespace AuthApiService.Models;

/// <summary>
/// API 응답을 나타내는 제네릭 레코드
/// </summary>
/// <typeparam name="T">응답 데이터 타입</typeparam>
/// <param name="Success">성공 여부</param>
/// <param name="Message">응답 메시지</param>
/// <param name="Data">응답 데이터</param>
/// <param name="Errors">에러 목록</param>
public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = default,
    IEnumerable<string>? Errors = null
)
{
    /// <summary>
    /// 성공 응답 생성
    /// </summary>
    /// <param name="data">응답 데이터</param>
    /// <param name="message">메시지</param>
    /// <returns>성공 응답</returns>
    public static ApiResponse<T> SuccessResponse(T data, string message = "성공")
        => new(true, message, data);

    /// <summary>
    /// 실패 응답 생성
    /// </summary>
    /// <param name="message">에러 메시지</param>
    /// <param name="errors">에러 목록</param>
    /// <returns>실패 응답</returns>
    public static ApiResponse<T> ErrorResponse(string message, IEnumerable<string>? errors = null)
        => new(false, message, default, errors);
}

/// <summary>
/// 인증 결과를 나타내는 레코드
/// </summary>
/// <param name="IsSuccess">인증 성공 여부</param>
/// <param name="User">인증된 사용자</param>
/// <param name="ErrorMessage">에러 메시지</param>
public record AuthResult(
    bool IsSuccess,
    ApplicationUser? User = null,
    string? ErrorMessage = null
)
{
    /// <summary>
    /// 성공 결과 생성
    /// </summary>
    /// <param name="user">인증된 사용자</param>
    /// <returns>성공 결과</returns>
    public static AuthResult Success(ApplicationUser user) => new(true, user);

    /// <summary>
    /// 실패 결과 생성
    /// </summary>
    /// <param name="errorMessage">에러 메시지</param>
    /// <returns>실패 결과</returns>
    public static AuthResult Failure(string errorMessage) => new(false, null, errorMessage);
}
