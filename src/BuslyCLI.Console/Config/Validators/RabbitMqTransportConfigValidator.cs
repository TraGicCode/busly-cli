using BuslyCLI.Config.Transports;
using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class RabbitMqTransportConfigValidator : AbstractValidator<RabbitmqTransportConfig>
{
    public RabbitMqTransportConfigValidator()
    {
        RuleFor(x => x.AmqpConnectionString)
            .NotEmpty();

        RuleFor(x => x.RoutingTopology)
            .IsInEnum();

        RuleFor(x => x.ManagementApi)
            .SetValidator(new ManagementApiConfigValidator());
    }
}

public class ManagementApiConfigValidator : AbstractValidator<ManagementApi>
{
    public ManagementApiConfigValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                    (string.IsNullOrEmpty(x.UserName) && string.IsNullOrEmpty(x.Password)) // both empty
                    || (!string.IsNullOrEmpty(x.UserName) && !string.IsNullOrEmpty(x.Password)) // both set
            )
            .WithMessage("Username and Password are mutually dependent: if one is set, the other must also be set.");
    }
}