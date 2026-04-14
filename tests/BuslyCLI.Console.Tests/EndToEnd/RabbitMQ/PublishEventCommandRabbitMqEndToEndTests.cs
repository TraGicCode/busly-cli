using System.Text.Json;
using BuslyCLI.Console.Tests.TestHelpers;

namespace BuslyCLI.Console.Tests.EndToEnd.RabbitMQ;

[TestFixture]
public class PublishEventCommandRabbitMqEndToEndTests : RabbitMqEndToEndTestBase
{
    [Test]
    public async Task ShouldPublishEvent()
    {
        // Arrange
        await TestEndpoint.Subscribe("MessageContracts.Events.OrderCreated");
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
            "event",
            "publish",
            "--content-type", "application/json",
            "--enclosed-message-type", "MessageContracts.Events.OrderCreated",
            "--message-body", json,
            "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        AssertMessageReceived(TestEndpoint.TryReceiveMessage(), "MessageContracts.Events.OrderCreated", json);
    }
}