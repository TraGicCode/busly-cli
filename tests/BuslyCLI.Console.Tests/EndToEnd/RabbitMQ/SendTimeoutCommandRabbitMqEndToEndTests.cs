using System.Text.Json;
using BuslyCLI.Console.Tests.TestHelpers;

namespace BuslyCLI.Console.Tests.EndToEnd.RabbitMQ;

[TestFixture]
public class SendTimeoutCommandRabbitMqEndToEndTests : RabbitMqEndToEndTestBase
{
    [Test]
    public async Task ShouldSendTimeoutWithDelayDeliveryWith()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };
        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        var yamlFile = $"""
                        ---
                        current-transport: local-rabbitmq
                        transports:
                          - name: local-rabbitmq
                            rabbitmq-transport-config:
                              amqp-connection-string: {Container.GetConnectionString()}
                              management-api:
                                url: http://{Container.Hostname}:{Container.GetMappedPublicPort(15672)}
                        """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run(
            "timeout",
            "send",
            "--content-type", "application/json",
            "--enclosed-message-type", "MessageContracts.Timeouts.OrderTimeout",
            "--destination-endpoint", TestEndpoint.EndpointName,
            "--message-body", json,
            "--delay-delivery-with", "00:00:01",
            "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        AssertMessageReceived(TestEndpoint.TryReceiveMessage(), "MessageContracts.Timeouts.OrderTimeout", json);
    }

    [Test]
    public async Task ShouldSendTimeoutWithDoNotDeliverBefore()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };
        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        var yamlFile = $"""
                        ---
                        current-transport: local-rabbitmq
                        transports:
                          - name: local-rabbitmq
                            rabbitmq-transport-config:
                              amqp-connection-string: {Container.GetConnectionString()}
                              management-api:
                                url: http://{Container.Hostname}:{Container.GetMappedPublicPort(15672)}
                        """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run(
            "timeout",
            "send",
            "--content-type", "application/json",
            "--enclosed-message-type", "MessageContracts.Timeouts.OrderTimeout",
            "--destination-endpoint", TestEndpoint.EndpointName,
            "--message-body", json,
            "--do-not-deliver-before", "2020-01-01T00:00:00Z",
            "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        AssertMessageReceived(TestEndpoint.TryReceiveMessage(), "MessageContracts.Timeouts.OrderTimeout", json);
    }
}