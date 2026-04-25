using HCMS.DoctorService.Domain.Handlers;
using HCMS.DoctorService.Infrastructure.Core;
using HCMS.DoctorService.Infrastructure.gRPC;
using HCMS.DoctorService.Infrastructure.Kafka.Consumers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5003");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddDbContext<ServiceDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddGrpc();

builder.Services.AddHostedService<KafkaConsumer>();

builder.Services.AddScoped<CreateSlotHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapGrpcService<DoctorGrpcService>();

app.Run();