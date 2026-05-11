using BuslyCLI.Console.Tests.TestHelpers;
using Spectre.Console.Testing;

namespace BuslyCLI.Console.Tests.Commands.ServiceControl.Instance;

public class CurrentServiceControlInstanceTests : CommandTestBase
{
    [Test]
    public void ShouldOutputCurrentInstance()
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
        var result = Sut.Run("servicecontrol", "instance", "current", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            """
            local-sc
            """.NormalizeLineEndings()
        ));
    }

    [Test]
    public void ShouldOutputNotSetWhenCurrentInstanceIsNotConfigured()
    {
        // Arrange
        var yamlFile = """
                       ---
                       current-service-control-instance:
                       service-control-instances:
                         - name: local-sc
                           url: http://localhost:33333
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run("servicecontrol", "instance", "current", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            """
            Current service control instance is not set.
            """.NormalizeLineEndings()
        ));
    }
}