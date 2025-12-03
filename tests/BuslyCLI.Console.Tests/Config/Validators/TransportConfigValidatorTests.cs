using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class TransportConfigValidatorTests
{
    private readonly TransportConfigValidator _validator;

    public TransportConfigValidatorTests()
    {
        _validator = new TransportConfigValidator();
    }

    [Test]
    public async Task ShouldNotErrorWhenCurrentNameIsDefined()
    {
        var config = new TransportConfig()
        {
            Name = "local-learning"
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Test]
    public async Task ShouldErrorWhenCurrentTransportIsNotDefined()
    {
        // Arrange
        var config = new TransportConfig()
        {
            Name = null
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name)
            .WithErrorMessage("'Name' must not be empty.");
    }
}