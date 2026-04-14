using BuslyCLI.Config.Transports;
using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class AzureStorageQueuesTransportConfigValidator : AbstractValidator<AzureStorageQueuesTransportConfig>
{
    public AzureStorageQueuesTransportConfigValidator()
    {
        RuleFor(x => x.ConnectionString)
            .NotEmpty();
    }
}