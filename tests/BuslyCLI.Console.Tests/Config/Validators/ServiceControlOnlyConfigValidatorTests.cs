using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class ServiceControlOnlyConfigValidatorTests
{
    private readonly ServiceControlOnlyConfigValidator _validator;

    public ServiceControlOnlyConfigValidatorTests()
    {
        _validator = new ServiceControlOnlyConfigValidator();
    }

    [Test]
    public async Task ShouldNotErrorWhenCurrentServiceControlInstanceIsDefined()
    {
        // Arrange
        var config = new NServiceBusConfig
        {
            CurrentServiceControlInstance = "sc-local",
            ServiceControlInstances = new List<ServiceControlInstanceConfig>()
            {
                new ServiceControlInstanceConfig { Name = "sc-local", Url = "http://localhost:33333/api/" }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CurrentServiceControlInstance);
    }

    [Test]
    public async Task ShouldErrorWhenServiceControlInstancesIsEmpty()
    {
        // Arrange
        var config = new NServiceBusConfig
        {
            CurrentServiceControlInstance = "sc-local",
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ServiceControlInstances)
            .WithErrorMessage("No service-control-instances are configured. Use \"busly servicecontrol instance set\" to add one.");
    }

    [Test]
    public async Task ShouldErrorWhenCurrentServiceControlInstanceIsNotSet()
    {
        // Arrange
        var config = new NServiceBusConfig
        {
            CurrentServiceControlInstance = "",
            ServiceControlInstances = new List<ServiceControlInstanceConfig>()
            {
                new ServiceControlInstanceConfig { Name = "sc-local", Url = "http://localhost:33333/api/" }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CurrentServiceControlInstance)
            .WithErrorMessage("No current-service-control-instance is set. Use \"busly servicecontrol instance set\" to select one.");
    }

    [Test]
    public async Task ShouldErrorWhenCurrentServiceControlInstanceDoesntMatchAnyConfiguredInstances()
    {
        // Arrange
        var config = new NServiceBusConfig
        {
            CurrentServiceControlInstance = "d",
            ServiceControlInstances = new List<ServiceControlInstanceConfig>()
            {
                new ServiceControlInstanceConfig { Name = "a", Url = "http://localhost:33333/api/" },
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(config);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CurrentServiceControlInstance)
            .WithErrorMessage("current-service-control-instance must match one of the defined service-control-instances.");
    }
}