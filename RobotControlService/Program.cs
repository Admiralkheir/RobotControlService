
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RobotControlService.Behaviors;
using RobotControlService.Data;
using RobotControlService.Features.Auth;
using RobotControlService.Middleware;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace RobotControlService
{
    public class Program
    {
        public static async Task Main(string[] args)
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
                           new string[] {}
                       }
                };

                o.AddSecurityRequirement(securityRequirement);
            });

            // Add services to the container.  
            builder.Services.AddSingleton<IAuthService, AuthService>();

            // Add ExceptionHandlingMiddleware  
            builder.Services.AddTransient<ExceptionHandlingMiddleware>();

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
                   // We can read from Azure Key Vault or other secure storage  
                   builder.Configuration["ApplicationInsights:ConnectionString"], // use TelemetryConfiguration  
                   TelemetryConverter.Traces)
               .CreateLogger();

            builder.Host.UseSerilog();

            // Add Application Insights telemetry  
            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                // We can read from Azure Key Vault or other secure storage  
                options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
            });

            builder.Services.AddControllers();
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                //});

            builder.Services.AddHttpContextAccessor();

            // Add health checks  
            builder.Services.AddHealthChecks();

            // Add FluentValidation  
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Add MediatR  
            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<Program>();
                // Pipeline behaviour for using fluentvalidations  
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

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.  
            if (app.Environment.IsDevelopment())
            {
                // For development seed admin user  
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<RobotDbContext>();
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                    await Seed.SeedAdminUserAsync(dbContext, authService, builder.Configuration);
                }

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapHealthChecks("/health");


            //app.UseHttpsRedirection();  

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
