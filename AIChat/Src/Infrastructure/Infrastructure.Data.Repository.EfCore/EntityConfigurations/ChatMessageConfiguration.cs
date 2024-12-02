using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Repository.EfCore.EntityConfigurations
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.Id)
                .HasColumnType("uuid")
                .ValueGeneratedNever();

            builder.Property(cm => cm.Question)
                .HasColumnType("text")
                .IsRequired();

            builder.Property(cm => cm.Answer)
                .HasColumnType("text");

            builder.HasOne(cm => cm.ChatSession)
                .WithMany(cs => cs.ChatMessages)
                .HasForeignKey(cm => cm.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("ChatMessages"); // Use plural form for table name for consistency
        }
    }

}
