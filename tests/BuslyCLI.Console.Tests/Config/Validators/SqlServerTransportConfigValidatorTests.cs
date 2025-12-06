using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class SqlServerTransportConfigValidatorTests
{
    private readonly SqlServerTransportConfigValidator _validator;

    public SqlServerTransportConfigValidatorTests()
    {
        _validator = new SqlServerTransportConfigValidator();
    }

    [Test]
    public async Task ShouldErrorWhenConnectionStringIsNotPassed()
    {
        // Arrange
        var sqlServerTransportConfig = new SqlServerTransportConfig
        {
            ConnectionString = null
        };
        // Act
        var result = await _validator.TestValidateAsync(sqlServerTransportConfig);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ConnectionString)
            .WithErrorMessage("'Connection String' must not be empty.");
    }

    [Test]
    public async Task ShouldNotErrorConnectionStringIsPassed()
    {
        // Arrange
        var sqlServerTransportConfig = new SqlServerTransportConfig
        {
            ConnectionString = "Data Source=(local);Initial Catalog=Ordering;Integrated Security=SSPI;Application Name=Busly-CLI;TrustServerCertificate=true"
        };
        // Act
        var result = await _validator.TestValidateAsync(sqlServerTransportConfig);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.ConnectionString);
    }
}