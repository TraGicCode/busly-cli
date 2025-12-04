using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class AzureServiceBusTransportConfigValidator : AbstractValidator<AzureServiceBusTransportConfig>
{
    public AzureServiceBusTransportConfigValidator()
    {
        RuleFor(x => x.ConnectionString)
            .NotEmpty();
    }
}