using map_tile_server.Filters;
using map_tile_server.Middlewares;
using map_tile_server.Models.Configurations;
using map_tile_server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Text;

namespace map_tile_server.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key")))
                };
            });
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.AddSingleton<IJwtSettings>(s => s.GetRequiredService<IOptions<JwtSettings>>().Value);
        }

        public static void ConfigureMongoDatabaseSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection("MongoDatabaseSettings"));
            services.AddSingleton<IDatabaseSettings>(s => s.GetRequiredService<IOptions<DatabaseSettings>>().Value);
            services.AddSingleton<IMongoClient>(s => new MongoClient(configuration.GetValue<string>("MongoDatabaseSettings:ConnectionString")));
        }

        public static void ConfigureMapTileServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TileServerSettings>(configuration.GetSection("TileServerSettings"));
            services.AddSingleton<ITileServerSettings>(s => s.GetRequiredService<IOptions<TileServerSettings>>().Value);
        }

        public static void ConfigureOsmDatabaseSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OsmDatabaseSettings>(configuration.GetSection("OsmDatabaseSettings"));
            services.AddSingleton<IOsmDatabaseSettings>(s => s.GetRequiredService<IOptions<OsmDatabaseSettings>>().Value);
        }
        public static void AddLogger(this IServiceCollection services, IHostBuilder host)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console(new JsonFormatter()).CreateLogger();
            try
            {
                host.UseSerilog((ctx, lc) => lc.WriteTo.Console());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static void UseConfigureCustomExceptionMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        public static void UseConfiguredCors(this WebApplication app)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilter>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IHelperService, HelperService>();
            services.AddSingleton<IMapService, MapService>();
            services.AddSingleton<IOsmService, OsmService>();
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
