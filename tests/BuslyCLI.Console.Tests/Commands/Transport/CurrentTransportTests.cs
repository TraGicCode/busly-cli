using BuslyCLI.Console.Tests.TestHelpers;
using Spectre.Console.Testing;

namespace BuslyCLI.Console.Tests.Commands.Transport;

public class CurrentTransportTests : CommandTestBase
{

    [Test]
    public void ShouldOutputCurrentTransport()
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
        var result = Sut.Run("transport", "current", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            """
                local-learning
                """.NormalizeLineEndings()
        ));
    }
    [Test]
    public void ShouldOutputCurrentTransportNotConfigured()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-transport:
                       transports:
                         - name: local-learning
                           learning-transport-config:
                             storage-directory: .learningtransport
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run("transport", "current", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            """
                Current transport is not set.
                """.NormalizeLineEndings()
        ));
    }
}