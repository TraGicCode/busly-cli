using FluentValidation;

namespace BuslyCLI.Config.Validators;

public class SqlServerTransportConfigValidator : AbstractValidator<SqlServerTransportConfig>
{
    public SqlServerTransportConfigValidator()
    {
        RuleFor(x => x.ConnectionString)
            .NotEmpty();
    }
}