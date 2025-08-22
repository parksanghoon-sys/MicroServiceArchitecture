﻿using JWTAuth.Service.Models;
using JWTAuth.Service.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace JWTAuth.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }
    }
}
