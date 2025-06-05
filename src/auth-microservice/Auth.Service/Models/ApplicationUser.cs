using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Models;

public class ApplicationUser : IdentityUser
{
    public string UserId { get; set; } = string.Empty;
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }    
    public string LastLoginIp { get; set; }
    
    public string RegistrationIp { get; set; }

    public bool IsActive { get; set; } = true;

}
