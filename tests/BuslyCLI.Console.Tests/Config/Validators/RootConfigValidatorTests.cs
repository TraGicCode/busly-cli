using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class RootConfigValidatorTests
{
    private readonly RootConfigValidator _validator;

    public RootConfigValidatorTests()
    {
        _validator = new RootConfigValidator();
    }

    [Test]
    public async Task ShouldNotErrorWhenCurrentTransportIsDefined()
    {
        // Arrange
        var config = new NServiceBusConfig
        {
            CurrentTransport = "local-learning",
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CurrentTransport);
    }

    [Test]
    public async Task ShouldErrorWhenCurrentTransportIsNotDefined()
    {
        // Arrange
        var config = new NServiceBusConfig
        {
            CurrentTransport = "",
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CurrentTransport)
            .WithErrorMessage("'Current Transport' must not be empty.");
    }

    [Test]
    public async Task ShouldErrorWhenCurrentTransportDoesntMatchAnyConfiguredTransports()
    {
        // Arrange
        var config = new NServiceBusConfig
        {
            CurrentTransport = "d",
            Transports = new List<TransportConfig>()
            {
                new TransportConfig { Name = "a" },
                new TransportConfig { Name = "b" }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CurrentTransport)
            .WithErrorMessage("current-transport must match one of the defined transports.");
    }

    [Test]
    public async Task ShouldErrorWhenTransportsArrayIsEmpty()
    {
        // Arrange
        var config = new NServiceBusConfig
        {
            CurrentTransport = "d",
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Transports)
            .WithErrorMessage("'Transports' must not be empty.");
    }

}