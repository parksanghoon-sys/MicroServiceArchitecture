using Auth.Service.Models;

namespace Auth.Service.ApiModels.Responses;

// 결과 모델들
public class AuthResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public ApplicationUser User { get; set; }
    public TokenResponseDto Tokens { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
