using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Repository.EfCore.EntityConfigurations
{
    public class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
    {
        public void Configure(EntityTypeBuilder<ChatSession> builder)
        {
            builder.HasKey(cs => cs.Id);

            builder.Property(cs => cs.Id)
                .HasColumnType("uuid")
                .ValueGeneratedNever();

            builder.Property(cs => cs.SessionName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(cs => cs.Description)
                .HasColumnType("text");

            builder.HasIndex(cs => cs.SessionName)
                .HasDatabaseName("IX_ChatSession_SessionName"); // Explicitly name the index

            builder.HasOne(cs => cs.ApplicationUser)
                .WithMany(u => u.ChatSessions)
                .HasForeignKey(cs => cs.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("ChatSession");
        }
    }
}
