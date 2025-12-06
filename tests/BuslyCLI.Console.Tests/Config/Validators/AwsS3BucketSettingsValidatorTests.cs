using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;


[TestFixture]
public class AwsS3BucketSettingsValidatorTests
{
    private readonly AwsS3BucketSettingsValidator _validator;

    public AwsS3BucketSettingsValidatorTests()
    {
        _validator = new AwsS3BucketSettingsValidator();
    }

    [Test]
    public async Task ShouldErrorWhenBucketNameIsNotPassed()
    {
        // Arrange
        var awsS3BucketSettings = new AwsS3BucketSettings()
        {
            BucketName = null
        };

        // Act
        var result = await _validator.TestValidateAsync(awsS3BucketSettings);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.BucketName)
            .WithErrorMessage("'Bucket Name' must not be empty.");
    }

    [Test]
    public async Task ShouldErrorWhenKeyPrefixIsNotPassed()
    {
        // Arrange
        var awsS3BucketSettings = new AwsS3BucketSettings()
        {
            KeyPrefix = null
        };

        // Act
        var result = await _validator.TestValidateAsync(awsS3BucketSettings);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.KeyPrefix)
            .WithErrorMessage("'Key Prefix' must not be empty.");
    }
}