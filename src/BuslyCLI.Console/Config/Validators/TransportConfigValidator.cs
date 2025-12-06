using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class TransportConfigValidator : AbstractValidator<TransportConfig>
{
    public TransportConfigValidator()
    {
        RuleFor(x => x.Name).NotEmpty();

        RuleFor(x => x.Config)
            .NotEmpty()
            .WithMessage("Transport must define exactly one transport configuration.")
            .SetInheritanceValidator(v =>
            {
                v.Add(new LearningTransportConfigValidator());
                v.Add(new RabbitMQTransportConfigValidator());
                v.Add(new AzureServiceBusTransportConfigValidator());
                v.Add(new AzureStorageQueuesTransportConfigValidator());
                v.Add(new AmazonsqsTransportConfigValidator());
                v.Add(new SqlServerTransportConfigValidator());
                v.Add(new PostgreSqlTransportConfigValidator());
            });

        // RuleFor(x => x.LearningTransportConfig)
        //     .SetValidator(new LearningTransportConfigValidator())
        //     .When(x => x.Config is not null);

    }
}