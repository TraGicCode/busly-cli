using BuslyCLI.Console.Tests.TestHelpers;
using Spectre.Console.Testing;

namespace BuslyCLI.Console.Tests.Commands.Transport;

public class DeleteTransportTests : CommandTestBase
{

    [Test]
    public void ShouldBeIdempotentWhenDeletingNonExistingTransport()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-transport: local-learning
                       transports:
                         - name: local-learning
                           learning-transport-config:
                             storage-directory: .learningtransport
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);
        var nonExistingTransport = Guid.NewGuid().ToString();

        // Act
        var result = Sut.Run("transport", "delete", nonExistingTransport, "--config", configFile.FilePath);

        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo($"Cannot delete transport {nonExistingTransport} since it doesn't exist in the config file."));
    }

    [Test]
    public void ShouldDeleteTransport()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-transport: local-learning
                       transports:
                         - name: local-learning
                           learning-transport-config:
                             storage-directory: .learningtransport
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run("transport", "delete", "local-learning", "--config", configFile.FilePath);

        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"""
                This removed your active transport, use "nservicebus transport set" to select a different one.
                deleted transport named local-learning from {configFile.FilePath}
                """.NormalizeLineEndings()
        ));
    }
}