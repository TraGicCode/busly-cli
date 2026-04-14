using System.Text.Json;
using BuslyCLI.Console.Tests.TestHelpers;

namespace BuslyCLI.Console.Tests.EndToEnd.Learning;

[TestFixture]
public class SendTimeoutCommandLearningEndToEndTests : LearningEndToEndTestBase
{
    [Test]
    public async Task ShouldSendTimeoutWithDelayDeliveryWith()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };
        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        var yamlFile = $"""
                        ---
                        current-transport: local-learning
                        transports:
                          - name: local-learning
                            learning-transport-config:
                              storage-directory: ./.learningtransport
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
                        current-transport: local-learning
                        transports:
                          - name: local-learning
                            learning-transport-config:
                              storage-directory: ./.learningtransport
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