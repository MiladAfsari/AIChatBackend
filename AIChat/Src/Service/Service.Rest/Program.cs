using Hangfire;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Service.Rest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration["ConnectionStrings:ApplicationDbConnection"]);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddOptions();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service.Rest", Version = "v1" });
                c.OperationFilter<SwaggerFileOperationFilter>();
            });

            builder.Services.RegisterMediatorService();
            builder.Services.RegisterIdentityAuthentication();
            builder.Services.RegisterRepositories();
            builder.Services.RegisterUnitOfWorks();
            builder.Services.RegisterTokenService();
            builder.Services.RegisterJWTAuthentication(builder.Configuration);
            builder.Services.RegisterHangfireService(builder.Configuration);

            // Register ILogger
            builder.Services.AddLogging();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(cfg => { cfg.MapControllers(); });

            //app.UseHangfireDashboard();
            app.Run();
        }
    }
}
