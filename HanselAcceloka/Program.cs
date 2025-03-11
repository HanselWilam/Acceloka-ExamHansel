using HanselAcceloka.Entities;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using FluentValidation.AspNetCore;
using HanselAcceloka.Services;

var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logDirectory))
{
    Directory.CreateDirectory(logDirectory);
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"logs/Log-{DateTime.UtcNow:yyyyMMdd}.txt")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var configuration = builder.Configuration;

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntityFrameworkNpgsql();
builder.Services.AddDbContext<AccelokaContext>(options =>
{
    var conString = configuration.GetConnectionString("PostgreDB");
    options.UseNpgsql(conString);
});

builder.Services.AddScoped<BookedTicketService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseRouting();
app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();