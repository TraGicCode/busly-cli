using Bogus;
using BuslyCLI.Config.Transports;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class LearningTransportConfigValidatorTests
{
    private readonly LearningTransportConfigValidator _validator;

    public LearningTransportConfigValidatorTests()
    {
        _validator = new LearningTransportConfigValidator();
    }

    [Test]
    public async Task ShouldErrorWhenStorageDirectoryIsNotPassed()
    {
        // Arrange
        var learningTransportConfig = new LearningTransportConfig
        {
            StorageDirectory = null
        };
        // Act
        var result = await _validator.TestValidateAsync(learningTransportConfig);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.StorageDirectory)
            .WithErrorMessage("'Storage Directory' must not be empty.");
    }

    [Test]
    public async Task ShouldNotErrorStorageDirectoryIsPassed()
    {
        // Arrange
        var learningTransportConfig = new LearningTransportConfig
        {
            StorageDirectory = new Faker().System.DirectoryPath()
        };
        // Act
        var result = await _validator.TestValidateAsync(learningTransportConfig);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.StorageDirectory);
    }
}