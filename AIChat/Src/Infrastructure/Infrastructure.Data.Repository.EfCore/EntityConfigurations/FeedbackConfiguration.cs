using Domain.Core.Entities.FeedbackTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Repository.EfCore.EntityConfigurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .HasColumnType("uuid")
                .ValueGeneratedNever();

            builder.Property(f => f.IsLiked)
                .IsRequired();

            builder.HasIndex(f => f.IsLiked); // Index for faster searches by IsLiked

            builder.HasOne(f => f.ChatMessage)
                .WithOne(cm => cm.Feedback)
                .HasForeignKey<Feedback>(f => f.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Feedback");
        }
    }
}
