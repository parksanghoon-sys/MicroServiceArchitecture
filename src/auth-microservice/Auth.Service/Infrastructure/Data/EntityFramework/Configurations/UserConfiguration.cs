using Auth.Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Auth.Service.Infrastructure.Data.EntityFramework.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            builder.Property(p => p.Email).IsRequired();
            builder.Property(p => p.UserId).IsRequired();
            // IPv6 최대 길이
            builder.Property(p => p.LastLoginIp).HasMaxLength(45);
            builder.Property(p => p.RegistrationIp).HasMaxLength(45);
            //builder.Property(p => p.LastName).IsRequired();

            builder.HasData(
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    UserId = "Admin123",
                    Email = "admin@localhost.com",
                    NormalizedEmail = "ADMIN@LOCALHOST.COM",
                    UserName = "admin@localhost.com",
                    NormalizedUserName = "ADMIN@LOCALHOST.COM",
                    PasswordHash = hasher.HashPassword(null, "Password!@123"),
                    EmailConfirmed = true                    
                },
                new ApplicationUser
                {
                    Id = "9e224968-33e4-4652-b7b7-8574d048cdb9",
                    UserId = "User123",
                    Email = "user@localhost.com",
                    NormalizedEmail = "USER@LOCALHOST.COM",                 
                    UserName = "user@localhost.com",
                    NormalizedUserName = "USER@LOCALHOST.COM",
                    PasswordHash = hasher.HashPassword(null, "Password!@123"),
                    EmailConfirmed = true
                }
                );
        }
    }
}