using BuslyCLI.Console.Tests.TestHelpers;
using Spectre.Console.Testing;

namespace BuslyCLI.Console.Tests.Commands.ServiceControl.Instance;

public class DeleteServiceControlInstanceTests : CommandTestBase
{
    [Test]
    public void ShouldBeIdempotentWhenDeletingNonExistingInstance()
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
        var nonExistingInstance = Guid.NewGuid().ToString();

        // Act
        var result = Sut.Run("servicecontrol", "instance", "delete", nonExistingInstance, "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"Cannot delete service control instance {nonExistingInstance} since it doesn't exist in the config file."
        ));
    }

    [Test]
    public void ShouldDeleteInstance()
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
        var result = Sut.Run("servicecontrol", "instance", "delete", "local-sc", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"""
             This removed your active service control instance, use "busly servicecontrol instance set" to select a different one.
             Deleted service control instance named local-sc from {configFile.FilePath}
             """.NormalizeLineEndings()
        ));
    }

    [Test]
    public void ShouldDeleteNonCurrentInstance()
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
        var result = Sut.Run("servicecontrol", "instance", "delete", "staging-sc", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"Deleted service control instance named staging-sc from {configFile.FilePath}"
        ));
    }
}