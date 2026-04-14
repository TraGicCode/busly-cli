using System.Text.Json;
using BuslyCLI.Console.Tests.TestHelpers;

namespace BuslyCLI.Console.Tests.EndToEnd.AzureStorageQueues;

[TestFixture]
public class SendTimeoutCommandAzureStorageQueuesEndToEndTests : AzureStorageQueuesEndToEndTestBase
{
    [Test]
    public async Task ShouldReturnErrorWhenSendingTimeoutWithDelayDeliveryWith()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };
        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        var yamlFile = $"""
                        ---
                        current-transport: local-azure-storage-queues
                        transports:
                          - name: local-azure-storage-queues
                            azure-storage-queues-transport-config:
                              connection-string: {Container.GetConnectionString()}
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
        Assert.That(result.ExitCode, Is.EqualTo(1));
        Assert.That(TestEndpoint.TryReceiveMessage(), Is.Null);
    }

    [Test]
    public async Task ShouldReturnErrorWhenSendingTimeoutWithDoNotDeliverBefore()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };
        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        var yamlFile = $"""
                        ---
                        current-transport: local-azure-storage-queues
                        transports:
                          - name: local-azure-storage-queues
                            azure-storage-queues-transport-config:
                              connection-string: {Container.GetConnectionString()}
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
        Assert.That(result.ExitCode, Is.EqualTo(1));
        Assert.That(TestEndpoint.TryReceiveMessage(), Is.Null);
    }
}