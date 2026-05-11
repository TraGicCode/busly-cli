using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;
using TransportConfig = BuslyCLI.Config.TransportConfig;

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
    public async Task ShouldNotErrorWhenNameIsDefined()
    {
        // Arrange
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
    public async Task ShouldErrorWhenNameIsNotDefined()
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