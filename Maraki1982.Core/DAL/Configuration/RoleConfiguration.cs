using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Maraki1982.Core.DAL.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public string RoleId = Guid.NewGuid().ToString();

        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(new IdentityRole
            {
                Name = "Super Admin",
                NormalizedName = "Super Admin".ToUpper(),
                Id = RoleId,
                ConcurrencyStamp = RoleId
            });
        }
    }
}
