using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class AmazonsqsTransportConfigValidatorTests
{
    private readonly AmazonsqsTransportConfigValidator _validator;

    public AmazonsqsTransportConfigValidatorTests()
    {
        _validator = new AmazonsqsTransportConfigValidator();
    }

    [Test]
    public async Task ShouldErrorWhenRegionNameIsNotPassed()
    {
        // Arrange
        var amazonsqsTransportConfig = new AmazonsqsTransportConfig
        {
            RegionName = null
        };

        // Act
        var result = await _validator.TestValidateAsync(amazonsqsTransportConfig);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.RegionName)
            .WithErrorMessage("'Region Name' must not be empty.");
    }

    [Test]
    public async Task ShouldNotErrorWhenRegionNameIsPassed()
    {
        // Arrange
        var amazonsqsTransportConfig = new AmazonsqsTransportConfig
        {
            RegionName = "us-east-1"
        };
        // Act
        var result = await _validator.TestValidateAsync(amazonsqsTransportConfig);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.RegionName);
    }

    [Test]
    public async Task ShouldErrorWhenServiceUrlIsNotPassed()
    {
        // Arrange
        var amazonsqsTransportConfig = new AmazonsqsTransportConfig
        {
            RegionName = null
        };

        // Act
        var result = await _validator.TestValidateAsync(amazonsqsTransportConfig);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ServiceUrl)
            .WithErrorMessage("'Service Url' must not be empty.");
    }

    [Test]
    public async Task ShouldNotErrorWhenServiceUrlIsPassed()
    {
        // Arrange
        var amazonsqsTransportConfig = new AmazonsqsTransportConfig
        {
            ServiceUrl = "us-east-1"
        };
        // Act
        var result = await _validator.TestValidateAsync(amazonsqsTransportConfig);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.ServiceUrl);
    }
}