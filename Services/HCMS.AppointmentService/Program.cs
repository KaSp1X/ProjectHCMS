using DoctorServiceGrpc;
using HCMS.AppointmentService.Domain.Handlers;
using HCMS.AppointmentService.Infrastructure.Core;
using HCMS.AppointmentService.Infrastructure.Kafka;
using HCMS.AppointmentService.Workers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5001");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddDbContext<ServiceDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddGrpcClient<DoctorAvailability.DoctorAvailabilityClient>(o =>
{
    o.Address = new Uri("http://localhost:5002");
});

builder.Services.AddHostedService<OutboxWorker>();

builder.Services.AddSingleton<KafkaProducer>();

builder.Services.AddScoped<CreateAppointmentHandler>();
builder.Services.AddScoped<GetAppointmentsByDoctorHandler>();
builder.Services.AddScoped<GetAllAppointmentsHandler>();
builder.Services.AddScoped<CancelAppointmentHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
