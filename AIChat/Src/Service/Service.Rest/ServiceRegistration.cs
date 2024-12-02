using Application.Command.Base;
using Application.Query.Base;
using Application.Service.Common;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.Entities.FeedbackTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.Exception;
using Domain.Core.UnitOfWorkContracts;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Infrastructure.Data.Repository.EfCore.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.MediatR;
using System.Reflection;
using System.Text;

namespace Service.Rest
{
    public static class ServiceRegistration
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
            services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
            services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();
        }
        public static void RegisterUnitOfWorks(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDbContextUnitOfWork, ApplicationDbContextUnitOfWork>();
        }
        public static void RegisterMediatorService(this IServiceCollection services)
        {
            services.AddMediatorService(options =>
            {
                options.SetAssemblies(new Assembly[]
                {
                    Assembly.GetAssembly(typeof(BaseCommandHandler)),
                    Assembly.GetAssembly(typeof(BaseQueryHandler))
                });
                options.EnableAutoLogging = false;
                options.EnableAutoValidation = true;
            });
        }
        public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        }
        public static void RegisterTokenService(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}
