using Auth.Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Service.Infrastructure.Data.EntityFramework
{
    internal class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(p => p.Email).IsRequired();
            builder.Property(p => p.UserId).IsRequired();
            //builder.Property(p => p.LastName).IsRequired();


        }
    }
}