using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class ServiceControlOnlyConfigValidator : AbstractValidator<NServiceBusConfig>
{
    public ServiceControlOnlyConfigValidator()
    {
        RuleFor(x => x.ServiceControlInstances)
            .NotEmpty()
            .WithMessage("No service-control-instances are configured. Use \"busly servicecontrol instance set\" to add one.")
            .ForEach(x => x.SetValidator(new ServiceControlInstanceConfigValidator()));

        RuleFor(x => x.CurrentServiceControlInstance)
            .NotEmpty()
            .WithMessage("No current-service-control-instance is set. Use \"busly servicecontrol instance set\" to select one.");

        RuleFor(x => x.CurrentServiceControlInstance)
            .Must((model, current) =>
                model.ServiceControlInstances != null &&
                model.ServiceControlInstances.Any(s => s.Name == current))
            .WithMessage("current-service-control-instance must match one of the defined service-control-instances.")
            .When(x => !string.IsNullOrEmpty(x.CurrentServiceControlInstance));
    }
}