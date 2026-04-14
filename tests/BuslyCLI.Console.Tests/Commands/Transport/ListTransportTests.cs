using BuslyCLI.Console.Tests.TestHelpers;
using Spectre.Console.Testing;

namespace BuslyCLI.Console.Tests.Commands.Transport;

public class ListTransportTests : CommandTestBase
{

    [Test]
    public void ShouldOutputAnEmptyGridWhenConfigFileIsEmptyYaml()
    {
        // Arrange
        var yamlFile = """
                       ---
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);
        var result = Sut.Run("transport", "list", "--config", configFile.FilePath);

        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo("CURRENT  NAME  TRANSPORT-TYPE"));
    }

    [Test]
    public void ShouldOutputAnEmptyGridWhenTransportArrayIsEmpty()
    {
        // Arrange
        var yamlFile = """
                       ---
                       transports:
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);
        var result = Sut.Run("transport", "list", "--config", configFile.FilePath);

        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo("CURRENT  NAME  TRANSPORT-TYPE"));
    }
    [Test]
    public void ShouldOutputASingleTransport()
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
        var result = Sut.Run("transport", "list", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            """
            CURRENT  NAME            TRANSPORT-TYPE
            *        local-learning  learning
            """.NormalizeLineEndings()
        ));
    }
}