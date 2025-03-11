using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Service.Rest.Middlewares;

namespace Service.Rest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            try
            {
                builder.Services.AddPostgresDbContext(builder.Configuration);

                builder.Services.AddHttpContextAccessor();
                builder.Services.AddOptions();

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.RegisterSwaggerService();

                builder.Services.RegisterMediatorService();
                builder.Services.RegisterIdentityAuthentication();
                builder.Services.RegisterRepositories();
                builder.Services.RegisterUnitOfWorks();
                builder.Services.RegisterTokenService();
                builder.Services.AddChatBotInfrastructure(builder.Configuration);
                builder.Services.RegisterJWTAuthentication(builder.Configuration);
                builder.Services.RegisterHangfireService(builder.Configuration);

                // Register ILogger
                builder.Services.AddLogging();

                // Configure Serilog
                builder.ConfigureSerilog();

                // Add CORS policy to allow any origin
                builder.Services.AddCorsPolicy();

                builder.Services.AddControllers(options =>
                {
                    options.Filters.Add<TokenValidationFilter>();
                });

                var app = builder.Build();

                app.ApplyMigrations();
                
                //if (app.Environment.IsDevelopment())
                //{
                app.UseSwagger();
                app.UseSwaggerUI();
                //}
                
                app.UseRouting();
                app.UseCors("AllowAll");
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseMiddleware<LogRequestResponseMiddleware>();
                app.UseEndpoints(cfg => { cfg.MapControllers(); });

                //app.UseHangfireDashboard();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
