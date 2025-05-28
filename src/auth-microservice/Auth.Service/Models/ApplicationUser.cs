using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Models;

public class ApplicationUser : IdentityUser
{    
    public string? UserId { get ; set; }

}
