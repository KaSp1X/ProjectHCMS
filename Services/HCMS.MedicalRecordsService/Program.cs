using AppointmentServiceGrpc;
using HCMS.MedicalRecordsService.Infrastructure.Auth;
using HCMS.MedicalRecordsService.Infrastructure.Core;
using HCMS.MedicalRecordsService.Infrastructure.Kafka;
using HCMS.MedicalRecordsService.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => {
    options.ListenAnyIP(8080);
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

builder.Services.AddGrpcClient<AppointmentExistence.AppointmentExistenceClient>(o =>
{
    o.Address = new Uri("http://appointment-service:8081");
});

builder.Services.AddHostedService<OutboxWorker>();

builder.Services.AddSingleton<KafkaProducer>();

builder.Services.AddSingleton<ServiceDbContext>();

builder.Services.AddScoped<RecordAccessService>();

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();