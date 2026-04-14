using BuslyCLI.Config.Transports;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class AzureServiceBusTransportConfigValidatorTests
{
    private readonly AzureServiceBusTransportConfigValidator _validator;

    public AzureServiceBusTransportConfigValidatorTests()
    {
        _validator = new AzureServiceBusTransportConfigValidator();
    }

    [Test]
    public async Task ShouldErrorWhenConnectionStringIsNotPassed()
    {
        // Arrange
        var azureServiceBusTransportConfig = new AzureServiceBusTransportConfig
        {
            ConnectionString = null
        };
        // Act
        var result = await _validator.TestValidateAsync(azureServiceBusTransportConfig);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ConnectionString)
            .WithErrorMessage("'Connection String' must not be empty.");
    }

    [Test]
    public async Task ShouldNotErrorConnectionStringIsPassed()
    {
        // Arrange
        var azureServiceBusTransportConfig = new AzureServiceBusTransportConfig
        {
            ConnectionString = "Endpoint=amqp://127.0.0.1:32799/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true"
        };
        // Act
        var result = await _validator.TestValidateAsync(azureServiceBusTransportConfig);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.ConnectionString);
    }
}