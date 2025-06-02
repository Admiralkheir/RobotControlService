
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RobotControlService.Behaviors;
using RobotControlService.Data;
using RobotControlService.Middleware;
using Serilog;
using System.Reflection;
using System.Text;

namespace RobotControlService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // swagger configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(o =>
            {
                o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token in this field",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                };

                o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        []
                    }
                };

                o.AddSecurityRequirement(securityRequirement);
            });

            // Add versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Read version from URL segment
            })
            //.AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V"; // Version format in Swagger UI
                options.SubstituteApiVersionInUrl = true; // Substitute version in URL
            });

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
                options.UseMongoDB(builder.Configuration["MongoConnection:ConnectionURI"]!, builder.Configuration["MongoConnection:DatabaseName"]!)
            );

            // Register JWT authentication and authorization 
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ClockSkew = TimeSpan.Zero
                    };
                });

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

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
