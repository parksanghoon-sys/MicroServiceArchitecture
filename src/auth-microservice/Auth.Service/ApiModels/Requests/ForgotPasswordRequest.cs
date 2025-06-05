using System.ComponentModel.DataAnnotations;

namespace Auth.Service.ApiModels.Requests;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
