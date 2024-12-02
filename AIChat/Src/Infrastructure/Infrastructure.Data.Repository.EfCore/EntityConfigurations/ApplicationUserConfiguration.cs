using Domain.Core.Entities.UserTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Repository.EfCore.EntityConfigurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FullName)
            .HasMaxLength(150)
            .IsRequired();

            builder.HasIndex(u => u.FullName); 

            builder.ToTable("AspNetUsers");
        }
    }
}
