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

    [Test]
    public async Task ShouldErrorWhenAccessKeyIsPassedWithoutSecretKey()
    {
        // Arrange
        var amazonsqsTransportConfig = new AmazonsqsTransportConfig()
        {
            AccessKey = "BLAHBLAHBLAH",
            SecretKey = null
        };
        // Act
        var result = await _validator.TestValidateAsync(amazonsqsTransportConfig);

        // Assert
        result.ShouldHaveValidationErrors().WithErrorMessage("AWS AccessKey and SecretKey are mutually dependent: if one is set, the other must also be set.");
    }

    [Test]
    public async Task ShouldErrorWhenSecretIsPassedWithoutAccessKey()
    {
        // Arrange
        var amazonsqsTransportConfig = new AmazonsqsTransportConfig()
        {
            AccessKey = null,
            SecretKey = "BLAHBLAHBLAH"
        };
        // Act
        var result = await _validator.TestValidateAsync(amazonsqsTransportConfig);

        // Assert
        result.ShouldHaveValidationErrors().WithErrorMessage("AWS AccessKey and SecretKey are mutually dependent: if one is set, the other must also be set.");
    }
}