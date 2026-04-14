using BuslyCLI.Config.Transports;
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

        RuleFor(x => x.S3BucketSettings)
            .SetValidator(new AwsS3BucketSettingsValidator());
    }
}

public class AwsS3BucketSettingsValidator : AbstractValidator<AwsS3BucketSettings>
{
    public AwsS3BucketSettingsValidator()
    {
        RuleFor(x => x.BucketName)
            .NotEmpty();

        RuleFor(x => x.KeyPrefix)
            .NotEmpty();

    }
}