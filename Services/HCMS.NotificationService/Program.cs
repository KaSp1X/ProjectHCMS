using HCMS.NotificationService.Consumers;
using HCMS.NotificationService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<KafkaConsumer>();

builder.Services.AddScoped<NotificationHandler>();

var host = builder.Build();
host.Run();
