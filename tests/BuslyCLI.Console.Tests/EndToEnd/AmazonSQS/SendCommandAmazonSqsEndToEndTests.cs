using System.Text.Json;
using BuslyCLI.Console.Tests.TestHelpers;

namespace BuslyCLI.Console.Tests.EndToEnd.AmazonSQS;

[TestFixture]
public class SendCommandAmazonSqsEndToEndTests : AmazonSqsEndToEndTestBase
{
    [Test]
    public async Task ShouldSendCommand()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };

        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        var yamlFile = $"""
                        ---
                        current-transport: local-amazonsqs
                        transports:
                          - name: local-amazonsqs
                            amazonsqs-transport-config:
                              service-url: {Container.GetConnectionString()}
                              region-name: us-east-1
                        """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run(
            "command",
            "send",
            "--content-type", "application/json",
            "--enclosed-message-type", "MessageContracts.Commands.CreateOrder",
            "--destination-endpoint", TestEndpoint.EndpointName,
            "--message-body", json,
            "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        AssertMessageReceived(TestEndpoint.TryReceiveMessage(), "MessageContracts.Commands.CreateOrder", json);
    }

    [Test]
    public async Task ShouldPublishEvent()
    {
        // Arrange
        await TestEndpoint.Subscribe("MessageContracts.Events.OrderCreated");
        var messageBody = new { OrderNumber = Guid.NewGuid() };

        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        var yamlFile = $"""
                        ---
                        current-transport: local-amazonsqs
                        transports:
                          - name: local-amazonsqs
                            amazonsqs-transport-config:
                              service-url: {Container.GetConnectionString()}
                              region-name: us-east-1
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