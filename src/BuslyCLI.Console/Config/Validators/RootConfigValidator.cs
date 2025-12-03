using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class RootConfigValidator : AbstractValidator<NServiceBusConfig>
{
    public RootConfigValidator()
    {
        RuleFor(x => x.CurrentTransport).NotEmpty();

        RuleFor(x => x.Transports)
            .NotEmpty()
            .ForEach(x => x.SetValidator(new TransportConfigValidator()));

        RuleFor(x => x.CurrentTransport)
            .Must((model, currentTransport) =>
                model.Transports.Any(t => t.Name == currentTransport))
            .WithMessage("current-transport must match one of the defined transports.")
            .When(x => x.Transports != null);

    }
}