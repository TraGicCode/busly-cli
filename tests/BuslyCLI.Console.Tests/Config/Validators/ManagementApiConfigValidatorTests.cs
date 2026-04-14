using Bogus;
using BuslyCLI.Config.Transports;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class ManagementApiConfigValidatorTests
{
    private readonly ManagementApiConfigValidator _validator;

    public ManagementApiConfigValidatorTests()
    {
        _validator = new ManagementApiConfigValidator();
    }

    [Test]
    public async Task ShouldNotErrorWhenOnlyAUrlStringIsPassed()
    {
        // Arrange
        var managementApi = new ManagementApi()
        {
            Url = "http://localhost:15672"
        };
        // Act
        var result = await _validator.TestValidateAsync(managementApi);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Url);
    }

    [Test]
    public async Task ShouldNotErrorWhenOnlyCredentialsAreIsPassed()
    {
        // Arrange
        var managementApi = new ManagementApi()
        {
            UserName = new Faker().Internet.UserName(),
            Password = new Faker().Internet.Password()
        };
        // Act
        var result = await _validator.TestValidateAsync(managementApi);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task ShouldNotErrorWhenUrlAndCredentialsAreIsPassed()
    {
        // Arrange
        var managementApi = new ManagementApi()
        {
            Url = "http://localhost:15672",
            UserName = new Faker().Internet.UserName(),
            Password = new Faker().Internet.Password()
        };
        // Act
        var result = await _validator.TestValidateAsync(managementApi);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task ShouldErrorWhenUserNameIsPassedWithoutPassword()
    {
        // Arrange
        var managementApi = new ManagementApi()
        {
            UserName = new Faker().Internet.UserName()
        };
        // Act
        var result = await _validator.TestValidateAsync(managementApi);

        // Assert
        result.ShouldHaveValidationErrors().WithErrorMessage("Username and Password are mutually dependent: if one is set, the other must also be set.");
    }
}