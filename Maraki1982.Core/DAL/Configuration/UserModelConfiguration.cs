using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Maraki1982.Core.DAL.Configuration
{
    public class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public string UserId = Guid.NewGuid().ToString();

        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            var email = "super@admin.com";
            var password = "Passw0rd!";
            var hasher = new PasswordHasher<UserModel>();

            var userModel = new UserModel
            {
                Id = UserId,
                Email = email,
                EmailConfirmed = true,
                UserName = email,
                PasswordHash = hasher.HashPassword(null, password),
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper()
            };

            builder.HasData(userModel);
        }
    }
}
