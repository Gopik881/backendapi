using Elixir.Application.Features.State.Commands.CreateState;
using Microsoft.Extensions.DependencyInjection;

namespace Elixir.Infrastructure.Extensions;

public static class MediatRServiceRegistration
{
    public static IServiceCollection AddMediatRConfigurations(this IServiceCollection services)
    {
        // Register MediatR for atleast one of the type within the assembly, rest of the types will be registered automatically
        // MediatR will find all your command handlers (like CreateUserCommandHandler, DeleteUserCommandHandler, UpdateUserCommandHandler)
        // and query handlers (like GetUserByIdQueryHandler) within the Elixir.Application project's assembly.
        // You don't need to repeat this registration for each individual command or query class.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateStateCommand).Assembly));

        return services;
    }
}
