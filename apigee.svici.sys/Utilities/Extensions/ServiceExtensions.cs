using api.svici.sys.Infrastructure.CdmsSvService.DbService;
using api.svici.sys.Infrastructure.CdmsSvService.RequestService;
using api.svici.sys.Infrastructure.CdmsSvService.ResponseService;
using api.svici.sys.Infrastructure.SV_FE_Services;
using api.svici.sys.Infrastructure.SVBO_Service;
using api.svici.sys.Utilities.Data.Constants;
using app.api.Infrastructure.Logger;
using app.api.Utilities.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace app.api.Utilities.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureAppSetting(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConnectionString>(configuration.GetSection("ConnectionStrings"));
        services.Configure<SystemSettings>(configuration.GetSection("SystemSettings"));
        services.Configure<LogSetting>(configuration.GetSection("LOGUrl"));
        services.Configure<WsseCredentials>(configuration.GetSection("WsseCredentials"));
    }

    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = new SystemSettings();
        configuration.Bind("SystemSettings", jwtConfiguration);

        X509Store store = new X509Store(StoreLocation.LocalMachine);
        X509Certificate2? certificate = new();
        try
        {
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = store.Certificates;
            X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, System.DateTime.Now, false);
            X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindByThumbprint, jwtConfiguration.Thumbprint, false);

            if (signingCert.Count != 0)
                certificate = signingCert[0];
            else
                certificate = null;
        }
        catch
        {
            certificate = null;
        }
        finally
        {
            store.Close();
        }

        SecurityKey key = new X509SecurityKey(certificate);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfiguration.Issuer,
                ValidAudience = jwtConfiguration.AudienceId,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            option.Events = new JWTEvents();
        });
    }

    public static void ConfigureLoggerService(this IServiceCollection services, IConfiguration configuration)
    {
        //Nlog
        //services.AddSingleton<IloggerManager, LoggerManager>();
        //services.AddLogging(loggingBuilder =>
        //{
        //    loggingBuilder.ClearProviders();
        //    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
        //    loggingBuilder.AddNLog();
        //});

        //Serilog
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink((IConfigurationRoot)configuration, environment!))
        .Enrich.WithProperty("Environment", environment!)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
    {
        return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"mpu-creditcard-api-sys-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
        };
    }

    public static void ConfigureOAuthTokenConsumption(IConfiguration configuration)
    {
        var issuer = configuration.GetSection("SystemSettings:Issurer").Value;
        var thumbprint = configuration.GetSection("SystemSettings:Thumbprint").Value;
        var audienceId = configuration.GetSection("SystemSettings:AudienceId").Value;

        X509Store store = new X509Store(StoreLocation.LocalMachine);
        X509Certificate2? certificate = new();
        try
        {
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = store.Certificates;
            X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, System.DateTime.Now, false);
            X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindByThumbprint, thumbprint, false);

            if (signingCert.Count != 0)
                certificate = signingCert[0];
            else
                certificate = null;
        }
        catch
        {
            certificate = null;
        }
        finally
        {
            store.Close();
        }

    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "SVICI Core  WebAPI", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
    }

    public static void ConfigureServiceRefResolvers(this IServiceCollection services)
    {
        services.AddTransient<ICdmsSvDbService, CdmsSvDbService>();
        services.AddScoped<ICdmsSvService, CdmsSvService>();
        services.AddScoped<ICdmsSvResponseService, CdmsSvResponseService>();
        services.AddScoped<ISvFE_DbService, SvFE_DbService>();
        services.AddScoped<ISV_BO_Service, SV_BO_Service>();
    }
}
