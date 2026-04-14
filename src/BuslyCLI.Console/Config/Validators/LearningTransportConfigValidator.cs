using BuslyCLI.Config.Transports;
using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class LearningTransportConfigValidator : AbstractValidator<LearningTransportConfig>
{
    public LearningTransportConfigValidator()
    {
        RuleFor(x => x.StorageDirectory)
            .NotEmpty();
    }
}