using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using N5.Api.Models;

namespace N5.Api.Data.Map
{
    public class PermissionMap : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.EmployeeForename).IsRequired().HasMaxLength(100);
            builder.Property(x => x.EmployeeSurname).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PermissionTypeId).IsRequired();
            builder.Property(x => x.PermissionDate).IsRequired();

            builder.HasOne(x => x.PermissionType)
                .WithMany(x=>x.Permissions)
                .HasForeignKey(x => x.PermissionTypeId);
        }
    }
}
