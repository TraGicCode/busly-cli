using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class RabbitMqTransportConfigValidatorTests
{
    private readonly RabbitMqTransportConfigValidator _validator;

    public RabbitMqTransportConfigValidatorTests()
    {
        _validator = new RabbitMqTransportConfigValidator();
    }

    [Test]
    public async Task ShouldErrorWhenAmqpConnectionStringIsNotPassed()
    {
        // Arrange
        var rabbitmqTransportConfig = new RabbitmqTransportConfig()
        {
            AmqpConnectionString = null
        };
        // Act
        var result = await _validator.TestValidateAsync(rabbitmqTransportConfig);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.AmqpConnectionString)
            .WithErrorMessage("'Amqp Connection String' must not be empty.");
    }

    [Test]
    public async Task ShouldNotErrorWhenAmqpConnectionStringIsPassed()
    {
        // Arrange
        var rabbitmqTransportConfig = new RabbitmqTransportConfig()
        {
            AmqpConnectionString = "amqp://localhost"
        };
        // Act
        var result = await _validator.TestValidateAsync(rabbitmqTransportConfig);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.AmqpConnectionString);
    }
}