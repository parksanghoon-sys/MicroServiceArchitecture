using Microsoft.AspNetCore.Identity;

namespace JWTAuth.Service.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
