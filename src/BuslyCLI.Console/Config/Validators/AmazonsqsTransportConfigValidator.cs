using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class AmazonsqsTransportConfigValidator : AbstractValidator<AmazonsqsTransportConfig>
{
    public AmazonsqsTransportConfigValidator()
    {
        RuleFor(x => x.RegionName)
            .NotEmpty();

        RuleFor(x => x.ServiceUrl)
            .NotEmpty();
    }
}