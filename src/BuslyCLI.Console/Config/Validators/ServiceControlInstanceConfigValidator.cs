using BuslyCLI.Config;
using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class ServiceControlInstanceConfigValidator : AbstractValidator<ServiceControlInstanceConfig>
{
    public ServiceControlInstanceConfigValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Url)
            .NotEmpty()
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("'{PropertyName}' must be a valid absolute URL.");
    }
}