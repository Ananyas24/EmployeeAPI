using EmployeeCrudApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Logs to the console
    .WriteTo.File("logs/employee-api-.log", rollingInterval: RollingInterval.Day)  // Logs to files, rolling daily
    .CreateLogger();

// Add services to the container.
builder.Host.UseSerilog(); // Use Serilog for logging



// Configure the DB Context
builder.Services.AddDbContext<EmployeeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    // Configure JSON serialization to preserve references
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });


// Add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

