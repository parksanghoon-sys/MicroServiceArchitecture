using Auth.Service.ApiModels;
using Auth.Service.Infrastructure.Data.EntityFramework;
using Auth.Service.Models;
using Auth.Service.Services.IService;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AuthContext authContext, 
            IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthService> logger)
        {
            _db = authContext;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task<bool> AssignRole(string email, string roleName)
        {            
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email!.ToLower().Equals(email.ToLower()));
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

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault
                        (u => u.UserId.Equals(loginRequestDto.UserId));

            if (user is not null)
            {
                bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (user is null || isValid is false)
                    return new LoginResponseDto(User: null, Token: "");
                
                var token = await _jwtTokenGenerator.GetTokenAsync(new TokenRequestDto(user.Email));

                UserDto userDto = new(UserId:user.UserId, Email: user.Email!, Name: user.UserName!, PhoneNumber: user.PhoneNumber);

                LoginResponseDto loginResponseDto = new(User: userDto, Token: token.Token);

                return loginResponseDto;
            }
            else
                return new LoginResponseDto(User: null, Token: "");
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                Email = registrationRequestDto.Email,
                PhoneNumber = registrationRequestDto.PhoneNumber,      
                UserName = registrationRequestDto.UserName,
                UserId = registrationRequestDto.UserId,
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if(result.Succeeded == true)
                {
                    var userToRetrun = _db.ApplicationUsers.First(u => u.Email!.Equals(registrationRequestDto.Email));
                    UserDto userDto = new(UserId:user.UserId, Email: user.Email!, Name: user.UserName, PhoneNumber: user.PhoneNumber);

                    return userDto.Email;
                }
                else
                {
                    return result.Errors.FirstOrDefault()?.Description ?? "Error";
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{nameof(Register)} Error {ex}");
            }
            return "Error Encountered";
        }
  
    }
}
