using FluentValidation;

namespace Elixir.Application.Features.Module.Commands.Update;

public class UpdateModuleCommandValidator : AbstractValidator<UpdateModuleCommand>
{
    public UpdateModuleCommandValidator()
    {
        RuleFor(x => x.UpdateModuleDto.ModuleName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.UpdateModuleDto.Description)
            .MaximumLength(500);

        RuleFor(x => x.UpdateModuleDto.ModuleURL)
            .NotEmpty()
            .MaximumLength(500);

        // Add more rules as per ModuleCreateDto properties and their length constraints
    }
}
