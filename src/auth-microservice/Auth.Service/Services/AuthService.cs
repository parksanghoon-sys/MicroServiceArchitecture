using Auth.Service.ApiModels;
using Auth.Service.Infrastructure.Data.EntityFramework;
using Auth.Service.Models;
using Auth.Service.Services.IService;
using Microsoft.AspNetCore.Identity;
using System;

namespace Auth.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AuthContext authContext, 
            IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _db = authContext;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _roleManager = roleManager;
        }
        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            throw new NotImplementedException();
        }
    }
}
