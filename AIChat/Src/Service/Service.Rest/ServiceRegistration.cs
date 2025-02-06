using Application.Command.Base;
using Application.Query.Base;
using Application.Service.Common;
using Domain.Core.ApiLog;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.Entities.FeedbackTemplateAggregate;
using Domain.Core.Entities.InvalidatedTokenTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.Entities.UserTokenTemplateAggregate;
using Domain.Core.Error;
using Domain.Core.Exception;
using Domain.Core.UnitOfWorkContracts;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Infrastructure.Data.Repository.EfCore.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Shared.MediatR;
using Swashbuckle.AspNetCore.SwaggerGen;
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
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();
            services.AddScoped<IInvalidatedTokenRepository, InvalidatedTokenRepository>();
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
            services.AddScoped<IApiLogRepository, ApiLogRepository>();
        }
        public static void RegisterUnitOfWorks(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDbContextUnitOfWork, ApplicationDbContextUnitOfWork>();
        }
        public static void RegisterSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service.Rest", Version = "v1" });
                c.OperationFilter<SwaggerFileOperationFilter>();

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
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
        public static void RegisterHangfireService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config.UsePostgreSqlStorage(configuration["ConnectionStrings:HangfireConnection"]));
            services.AddHangfireServer();
        }
        public static void RegisterIdentityAuthentication(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }
        public static void RegisterJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
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
        public static void ApplyMigrations(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<ApplicationDbContext>();
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while applying migrations.");
                    }
                }
            }
        }
        public static void RegisterTokenService(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder()
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .Build());
            });

            return services;
        }
        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var exceptionLogRepository = serviceProvider.GetRequiredService<IExceptionLogRepository>();
            var applicationDbContextUnitOfWork = serviceProvider.GetRequiredService<IApplicationDbContextUnitOfWork>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Sink(new ExceptionLogSink(exceptionLogRepository, applicationDbContextUnitOfWork))
                .CreateLogger();
        }

        public class ExceptionLogSink : ILogEventSink
        {
            private readonly IExceptionLogRepository _exceptionLogRepository;
            private readonly IApplicationDbContextUnitOfWork _applicationDbContextUnitOfWork;

            public ExceptionLogSink(IExceptionLogRepository exceptionLogRepository, IApplicationDbContextUnitOfWork applicationDbContextUnitOfWork)
            {
                _exceptionLogRepository = exceptionLogRepository;
                _applicationDbContextUnitOfWork = applicationDbContextUnitOfWork;
            }

            public void Emit(LogEvent logEvent)
            {
                var exceptionLog = new ExceptionLog
                {
                    ExceptionType = logEvent.Exception?.GetType().Name ?? "Log",
                    Message = logEvent.RenderMessage(),
                    StackTrace = logEvent.Exception?.StackTrace ?? string.Empty,
                    InnerException = logEvent.Exception?.InnerException?.Message,
                    Path = string.Empty,
                    QueryString = string.Empty,
                    StatusCode = 0,
                    ElapsedMilliseconds = 0,
                    Timestamp = logEvent.Timestamp.UtcDateTime
                };

                _exceptionLogRepository.LogExceptionAsync(exceptionLog).GetAwaiter().GetResult();
                _applicationDbContextUnitOfWork.SaveChangesAsync().GetAwaiter().GetResult();
            }
        }
    }
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile));

            if (fileParameters.Any())
            {
                operation.Parameters.Clear();
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["file"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                },
                                Required = new HashSet<string> { "file" }
                            }
                        }
                    }
                };
            }
        }
    }
}
