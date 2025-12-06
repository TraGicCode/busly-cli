using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class PostgreSqlTransportConfigValidatorTests
{
    private readonly PostgreSqlTransportConfigValidator _validator;

    public PostgreSqlTransportConfigValidatorTests()
    {
        _validator = new PostgreSqlTransportConfigValidator();
    }

    [Test]
    public async Task ShouldErrorWhenConnectionStringIsNotPassed()
    {
        // Arrange
        var postgreSqlTransportConfig = new PostgreSqlTransportConfig
        {
            ConnectionString = null
        };
        // Act
        var result = await _validator.TestValidateAsync(postgreSqlTransportConfig);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ConnectionString)
            .WithErrorMessage("'Connection String' must not be empty.");
    }

    [Test]
    public async Task ShouldNotErrorConnectionStringIsPassed()
    {
        // Arrange
        var postgreSqlTransportConfig = new PostgreSqlTransportConfig
        {
            ConnectionString = "Data Source=(local);Initial Catalog=Ordering;Integrated Security=SSPI;Application Name=Busly-CLI;TrustServerCertificate=true"
        };
        // Act
        var result = await _validator.TestValidateAsync(postgreSqlTransportConfig);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.ConnectionString);
    }
}