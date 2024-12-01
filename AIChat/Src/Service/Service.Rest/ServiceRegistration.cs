using Application.Command.Base;
using Application.Query.Base;
using Domain.Core.Entities.ChatMessageTemplateAggregate;
using Domain.Core.Entities.ChatSessionTemplateAggregate;
using Domain.Core.Entities.FeedbackTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.Exception;
using Domain.Core.UnitOfWorkContracts;
using Infrastructure.Data.Repository.EfCore.DatabaseContexts;
using Infrastructure.Data.Repository.EfCore.Repositories;
using Shared.MediatR;
using System.Reflection;

namespace Service.Rest
{
    internal static class ServiceRegistration
    {
        internal static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
            services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
            services.AddScoped<IExceptionLogRepository, ExceptionLogRepository>();
        }
        internal static void RegisterUnitOfWorks(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDbContextUnitOfWork, ApplicationDbContextUnitOfWork>();
        }
        internal static void RegisterMediatorService(this IServiceCollection services)
        {
            services.AddMediatorService(options =>
            {
                options.SetAssemblies(new Assembly[]
                {
                    Assembly.GetAssembly(typeof(BaseCommandHandler)),
                    Assembly.GetAssembly(typeof(BaseQueryHandler))
                }); ;
                options.EnableAutoLogging = false;
                options.EnableAutoValidation = true;
            });
        }
        internal static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

        }

    }
}
