using Autofac.Core;
using Hangfire;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Service.Rest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            try
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(builder.Configuration["ConnectionStrings:ApplicationDbConnection"]);
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                });

                builder.Services.AddHttpContextAccessor();
                builder.Services.AddOptions();

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.RegisterSwaggerService();

                builder.Services.RegisterMediatorService();
                builder.Services.RegisterIdentityAuthentication();
                builder.Services.RegisterRepositories();
                builder.Services.RegisterUnitOfWorks();
                builder.Services.RegisterTokenService();
                builder.Services.RegisterJWTAuthentication(builder.Configuration);
                builder.Services.RegisterHangfireService(builder.Configuration);

                // Register ILogger
                builder.Services.AddLogging();

                // Configure Serilog
                builder.ConfigureSerilog();

                // Add CORS policy to allow any origin
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicyBuilder()
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .Build());
                });

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

                app.UseMiddleware<ExceptionHandlerMiddleware>();
                app.UseRouting();
                app.UseCors("AllowAll");
                app.UseAuthentication();
                app.UseAuthorization();
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
