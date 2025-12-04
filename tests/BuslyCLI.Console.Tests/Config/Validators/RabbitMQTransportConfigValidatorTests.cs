using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class RabbitMQTransportConfigValidatorTests
{
    private readonly RabbitMQTransportConfigValidator _validator;

    public RabbitMQTransportConfigValidatorTests()
    {
        _validator = new RabbitMQTransportConfigValidator();
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