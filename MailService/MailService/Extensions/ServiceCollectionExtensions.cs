using MailService.Services.Integrations.SendGrid;
using MailService.Services.Messaging;
using MailService.Services.Options;
using MailService.Services.Services.Mail;

namespace MailService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSendGridEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<SendGridOptions>()
                .Bind(configuration.GetSection("SendGrid"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<IEmailService, SendGridEmailService>();
            return services;
        }

        public static IServiceCollection AddRabbitMqEmailConsumer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
            services.AddSingleton<RabbitMqConnectionManager>();
            services.AddHostedService<RabbitMqEmailConsumer>();
            return services;
        }
    }
}
