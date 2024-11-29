
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.RegisterMediatorService();
            builder.Services.RegisterRepositories();
            builder.Services.RegisterUnitOfWorks();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(cfg => { cfg.MapControllers(); });
            app.Run();
        }
    }
}
