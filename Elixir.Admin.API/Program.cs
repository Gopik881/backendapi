using Elixir.Admin.API.Endpoints;
using Elixir.Admin.API.Extensions;
using Elixir.Application.Common.SingleSession.Interface;
using Elixir.Application.Features.Authentication.Models;
using Elixir.Application.Interfaces.Services;
using Elixir.Infrastructure.Extensions;
using Elixir.Infrastructure.Middleware;
using Elixir.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Elixir.Admin.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        //Versioning Setup
        builder.Services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        // Add services to the container.
        builder.Services.AddAuthorization();
        //Database
        builder.Services.AddDBInfrastructureServices(builder.Configuration);
        //Logging
        builder.Services.AddLoggingInfrastructureServices(builder.Logging, builder.Configuration);
        //Add Fluent Validation
        builder.Services.AddFluentValidationConfigurations();
        //Add MediatR
        builder.Services.AddMediatRConfigurations();
        //Add Authentication
        builder.Services.AddAuthenticationInfrastructureServices(builder.Configuration);

        //Add Email Service
        builder.Services.AddEmailInfrastructureServices(builder.Configuration);

        //Add File Handling Service
        builder.Services.AddFileDataManagementServices();
       
        //Swagger
        builder.Services.AddSwaggerServices();
  

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        
        // Register Health Checks
        builder.Services.AddHealthChecks();

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        builder.Services.AddSingleton<IJwtService, JwtService>();
        builder.Services.AddSingleton<IEmailService, EmailService>();

        var app = builder.Build();
        //// Map a health check endpoint
        //app.MapGet("/health", () => Results.Ok("API is healthy!"));

        app.UseCors("AllowAllOrigins");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<SessionValidationMiddleware>();


        app.MapControllers();


        ////Register the Endpoints
        //app.RegisterStateEndpoints();
        //app.RegisterCountryEndpoints();
        //app.RegisterTelephoneCodeEndpoints();
        //app.RegisterCurrencyEndpoints();
        //app.RegisterUserEndpoints();
        //app.RegisterCompanyEndpoints();
        //app.RegisterMenuEndpoints();
        //app.RegisterUserGroupsEndpoints();
        //app.RegisterMasterModuleEndpoints();
        //app.RegisterCompanyUserEndpoints();
        //app.RegisterModuleMappingEndpoints();
        //app.RegisterCompany5TabEndpoints();
        //app.RegisterNotificationEndpoints();
        //app.RegisterClientEndpoints();
        //app.RegisterEmailEndpoints();
        //app.UseHttpsRedirection();
        //app.RegisterSystemPolicyEndpoints();
        //app.RegisterMetadataEndpoints();
        //app.RegisterBulkUploadStatusEndpoints();

        // Register middleware
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseMiddleware<AuthenticationMiddleware>();

       // app.MapControllers();

        //app.UseEndpoints(endpoints =>
        //{
        //    // Map controller endpoints
        //    endpoints.MapControllers();
        //});

       
        app.Run();
    }
}
