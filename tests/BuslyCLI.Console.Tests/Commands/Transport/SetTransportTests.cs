using BuslyCLI.Console.Tests.TestHelpers;
using Spectre.Console.Testing;

namespace BuslyCLI.Console.Tests.Commands.Transport;

public class SetTransportTests : CommandTestBase
{

    [Test]
    public void ShouldOutputAMessageWhenTransportDoesNotExist()
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
        var nonExistingTransport = $"{Guid.NewGuid():N}";

        // Act
        var result = Sut.Run("transport", "set", nonExistingTransport, "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"""
                No transport exists with the name {nonExistingTransport}.
                """.NormalizeLineEndings()
        ));
    }

    [Test]
    public void ShouldOutputAMessageWhenTransportIsSet()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-transport: local-learning
                       transports:
                         - name: local-learning
                           learning-transport-config:
                             storage-directory: .learningtransport
                         - name: local-learning2
                           learning-transport-config:
                             storage-directory: .learningtransport
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run("transport", "set", "local-learning2", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"""
                 Switched to transport "local-learning2".
                 """.NormalizeLineEndings()
        ));
    }

    [Test]
    public void ShouldBeIdempotentWhenSettingTransportToTheAlreadyConfiguredTransport()
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
        var result = Sut.Run("transport", "set", "local-learning", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"""
                 Switched to transport "local-learning".
                 """.NormalizeLineEndings()
        ));
    }
}