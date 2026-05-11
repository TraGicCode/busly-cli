using BuslyCLI.Console.Tests.TestHelpers;
using Spectre.Console.Testing;

namespace BuslyCLI.Console.Tests.Commands.ServiceControl.Instance;

public class ListServiceControlInstancesTests : CommandTestBase
{
    [Test]
    public void ShouldOutputAnEmptyGridWhenConfigFileIsEmptyYaml()
    {
        // Arrange
        var yamlFile = """
                       ---
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run("servicecontrol", "instance", "list", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo("CURRENT  NAME  URL"));
    }

    [Test]
    public void ShouldOutputAnEmptyGridWhenServiceControlInstancesArrayIsEmpty()
    {
        // Arrange
        var yamlFile = """
                       ---
                       service-control-instances:
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run("servicecontrol", "instance", "list", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo("CURRENT  NAME  URL"));
    }

    [Test]
    public void ShouldOutputASingleInstance()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-service-control-instance: local-sc
                       service-control-instances:
                         - name: local-sc
                           url: http://localhost:33333
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run("servicecontrol", "instance", "list", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            """
            CURRENT  NAME      URL
            *        local-sc  http://localhost:33333
            """.NormalizeLineEndings()
        ));
    }

    [Test]
    public void ShouldOutputMultipleInstances()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-service-control-instance: local-sc
                       service-control-instances:
                         - name: local-sc
                           url: http://localhost:33333
                         - name: staging-sc
                           url: http://staging:33333
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run("servicecontrol", "instance", "list", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            """
            CURRENT  NAME        URL
            *        local-sc    http://localhost:33333
                     staging-sc  http://staging:33333
            """.NormalizeLineEndings()
        ));
    }
}