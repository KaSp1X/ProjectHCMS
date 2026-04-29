using DoctorServiceGrpc;
using HCMS.AppointmentService.Domain.Handlers;
using HCMS.AppointmentService.Infrastructure.Auth;
using HCMS.AppointmentService.Infrastructure.Core;
using HCMS.AppointmentService.Infrastructure.gRPC;
using HCMS.AppointmentService.Infrastructure.Kafka;
using HCMS.AppointmentService.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => {
    options.ListenAnyIP(8080, o => o.Protocols = HttpProtocols.Http1);
    options.ListenAnyIP(8081, o => o.Protocols = HttpProtocols.Http2);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddDbContext<ServiceDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddGrpc();
builder.Services.AddGrpcClient<DoctorAvailability.DoctorAvailabilityClient>(o =>
{
    o.Address = new Uri("http://doctor-service:8081");
});

builder.Services.AddHostedService<OutboxWorker>();

builder.Services.AddSingleton<KafkaProducer>();

builder.Services.AddScoped<AppointmentAccessService>();

builder.Services.AddScoped<CreateAppointmentHandler>();
builder.Services.AddScoped<CancelAppointmentHandler>();
builder.Services.AddScoped<CompleteAppointmentHandler>();
builder.Services.AddScoped<GetAppointmentsByDoctorHandler>();
builder.Services.AddScoped<GetAppointmentsHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers(); 
app.MapGrpcService<AppointmentGrpcService>();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
