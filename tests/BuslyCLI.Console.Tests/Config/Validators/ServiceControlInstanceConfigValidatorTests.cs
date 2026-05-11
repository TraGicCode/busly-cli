using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

public class ServiceControlInstanceConfigValidatorTests
{
    private readonly ServiceControlInstanceConfigValidator _validator;

    public ServiceControlInstanceConfigValidatorTests()
    {
        _validator = new ServiceControlInstanceConfigValidator();
    }

    [Test]
    public async Task ShouldNotErrorWhenNameIsDefined()
    {
        // Arrange
        var config = new ServiceControlInstanceConfig()
        {
            Name = "local-learning"
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldErrorWhenNameIsNotDefined()
    {
        // Arrange
        var config = new ServiceControlInstanceConfig()
        {
            Name = null
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("'Name' must not be empty.");
    }

    [Test]
    public async Task ShouldNotErrorWhenUrlsDefined()
    {
        // Arrange
        var config = new ServiceControlInstanceConfig()
        {
            Url = "http://localhost:33333/api/"
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Url);
    }

    [Test]
    public async Task ShouldErrorWhenUrlIsNotDefined()
    {
        // Arrange
        var config = new ServiceControlInstanceConfig()
        {
            Url = null
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Url)
            .WithErrorMessage("'Url' must not be empty.");
    }
}