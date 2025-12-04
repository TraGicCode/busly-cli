using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class AmazonsqsTransportConfigValidator : AbstractValidator<AmazonsqsTransportConfig>
{
    public AmazonsqsTransportConfigValidator()
    {
        RuleFor(x => x.RegionName)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x =>
                    (string.IsNullOrEmpty(x.AccessKey) && string.IsNullOrEmpty(x.SecretKey)) // both empty
                    || (!string.IsNullOrEmpty(x.AccessKey) && !string.IsNullOrEmpty(x.SecretKey)) // both set
            )
            .WithMessage("AWS AccessKey and SecretKey are mutually dependent: if one is set, the other must also be set.");
    }
}