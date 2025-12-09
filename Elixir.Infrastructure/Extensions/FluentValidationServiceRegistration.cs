using Elixir.Application.Features.State.Commands.CreateState;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Infrastructure.Extensions;

public static class FluentValidationServiceRegistration
{
    public static IServiceCollection AddFluentValidationConfigurations(this IServiceCollection services)
    {
        // Register atleast one of the Validator type within the assembly, rest of the types will be registered automatically
        // Fluent Validator will find all your command Validator
        // You don't need to repeat this registration for each individual commands.

        services.AddValidatorsFromAssemblyContaining<CreateStateCommandValidator>();
        return services;
    }
}
