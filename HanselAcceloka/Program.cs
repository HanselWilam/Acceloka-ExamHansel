using HanselAcceloka.Entities;
using HanselAcceloka.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.AspNetCore;

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

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntityFrameworkNpgsql();
builder.Services.AddDbContext<AccelokaContext>(options =>
{
    var conString = configuration.GetConnectionString("PostgreDB");
    options.UseNpgsql(conString);
});

builder.Services.AddTransient<TicketService>();
builder.Services.AddTransient<BookTicketService>();
builder.Services.AddScoped<BookedTicketService>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseAuthorization();
app.MapControllers();

app.Run();
