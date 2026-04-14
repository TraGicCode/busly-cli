using System.Text.Json;
using BuslyCLI.Console.Tests.TestHelpers;

namespace BuslyCLI.Console.Tests.EndToEnd.SqlServer;

[TestFixture]
public class SendCommandSqlServerEndToEndTests : SqlServerEndToEndTestBase
{
    [Test]
    public async Task ShouldSendCommand()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };
        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        var yamlFile = $"""
                        ---
                        current-transport: local-sql-server
                        transports:
                          - name: local-sql-server
                            sql-server-transport-config:
                              connection-string: {Container.GetConnectionString()}
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

}