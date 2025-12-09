using Elixir.Application.Interfaces.Services;
using Elixir.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Infrastructure.Extensions
{
    public static class FileDataManagementServiceRegistration
    {
        public static IServiceCollection AddFileDataManagementServices(this IServiceCollection services)
        {
            services.AddScoped<IFileHandlingService, FileHandlingService>();
            services.AddScoped<IFileDataConverterService, FileDataConverterService>();
            return services;
        }
    }
}
