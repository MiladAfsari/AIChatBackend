using Domain.Core.ApiLog;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.Entities.FeedbackTemplateAggregate;
using Domain.Core.Entities.InvalidatedTokenTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.Entities.UserTokenTemplateAggregate;
using Domain.Core.Error;
using Domain.Core.Exception;
using Infrastructure.Data.Repository.EfCore.EntityConfigurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Data.Repository.EfCore.DatabaseContexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<ApiLog> ApiLogs { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<InvalidatedToken> InvalidatedTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations using IEntityTypeConfiguration
            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new ChatSessionConfiguration());
            modelBuilder.ApplyConfiguration(new ChatMessageConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
        }
    }
}
