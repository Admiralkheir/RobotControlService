
using RobotControlService.Behaviors;
using RobotControlService.Middleware;
using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using Serilog;

namespace RobotControlService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // SeriLog logging
            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(builder.Configuration) // Reads "Serilog" section from appsettings
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .WriteTo.ApplicationInsights(
                   builder.Configuration["ApplicationInsights:ConnectionString"], // Or use TelemetryConfiguration.Active
                   TelemetryConverter.Traces)
               .CreateLogger();

            builder.Host.UseSerilog();

            // Add Application Insights telemetry
            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add health checks
            builder.Services.AddHealthChecks();

            // Add FluentValidation
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Add MediatR
            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<Program>();
                // Pipeline behaviour for using fluentvalida6ations
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            // Add DbContext

            builder.Services.AddDbContext<RobotDbContext>(options =>
                options.UseMongoDB(builder.Configuration["MongoConnection:ConnectionURI"], builder.Configuration["MongoConnection:DatabaseName"])
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapHealthChecks("/health");
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
