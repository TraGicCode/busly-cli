using BuslyCLI.Config.Transports;
using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class PostgreSqlTransportConfigValidator : AbstractValidator<PostgreSqlTransportConfig>
{
    public PostgreSqlTransportConfigValidator()
    {
        RuleFor(x => x.ConnectionString)
            .NotEmpty();
    }
}