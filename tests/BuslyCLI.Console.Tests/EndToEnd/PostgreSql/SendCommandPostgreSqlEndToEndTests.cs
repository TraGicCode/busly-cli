using System.Text.Json;
using BuslyCLI.Console.Tests.TestHelpers;

namespace BuslyCLI.Console.Tests.EndToEnd.PostgreSql;


[TestFixture]
public class SendCommandPostgreSqlEndToEndTests : PostgreSqlEndToEndTestBase
{
    [Test]
    public async Task ShouldSendCommand()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };
        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        var yamlFile = $"""
                        ---
                        current-transport: local-postgre-sql
                        transports:
                          - name: local-postgre-sql
                            postgre-sql-transport-config:
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