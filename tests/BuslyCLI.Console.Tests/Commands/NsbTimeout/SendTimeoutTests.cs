using BuslyCLI.Console.Tests.TestHelpers;

namespace BuslyCLI.Console.Tests.Commands.NsbTimeout;

public class SendTimeoutTests : CommandTestBase
{
    [Test]
    public void ShouldOutputAnErrorWhenTransportIsSqlServer()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-transport: local-sql-server
                       transports:
                         - name: local-sql-server
                           sql-server-transport-config:
                             connection-string: Server=localhost;Database=test;
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run(
            "timeout", "send",
            "--content-type", "application/json",
            "--enclosed-message-type", "MessageContracts.Timeouts.OrderTimeout",
            "--destination-endpoint", "Sales",
            "--message-body", "{}",
            "--delay-delivery-with", "00:00:01",
            "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(1));
        Assert.That(result.Output, Does.Contain("SqlServerTransport"));
        Assert.That(result.Output, Does.Contain("does not support sending timeouts"));
        Assert.That(result.Output, Does.Contain("https://tragiccode.com/busly-cli/docs/cli-reference/timeout/send"));
    }

    [Test]
    public void ShouldOutputAnErrorWhenTransportIsPostgreSql()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-transport: local-postgresql
                       transports:
                         - name: local-postgresql
                           postgre-sql-transport-config:
                             connection-string: Host=localhost;Database=test;
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run(
            "timeout", "send",
            "--content-type", "application/json",
            "--enclosed-message-type", "MessageContracts.Timeouts.OrderTimeout",
            "--destination-endpoint", "Sales",
            "--message-body", "{}",
            "--delay-delivery-with", "00:00:01",
            "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(1));
        Assert.That(result.Output, Does.Contain("PostgreSqlTransport"));
        Assert.That(result.Output, Does.Contain("does not support sending timeouts"));
        Assert.That(result.Output, Does.Contain("https://tragiccode.com/busly-cli/docs/cli-reference/timeout/send"));
    }

    [Test]
    public void ShouldOutputAnErrorWhenTransportIsAzureStorageQueues()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-transport: local-azure-storage-queues
                       transports:
                         - name: local-azure-storage-queues
                           azure-storage-queues-transport-config:
                             connection-string: UseDevelopmentStorage=true
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run(
            "timeout", "send",
            "--content-type", "application/json",
            "--enclosed-message-type", "MessageContracts.Timeouts.OrderTimeout",
            "--destination-endpoint", "Sales",
            "--message-body", "{}",
            "--delay-delivery-with", "00:00:01",
            "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(1));
        Assert.That(result.Output, Does.Contain("AzureStorageQueuesTransport"));
        Assert.That(result.Output, Does.Contain("does not support sending timeouts"));
        Assert.That(result.Output, Does.Contain("https://tragiccode.com/busly-cli/docs/cli-reference/timeout/send"));
    }
}