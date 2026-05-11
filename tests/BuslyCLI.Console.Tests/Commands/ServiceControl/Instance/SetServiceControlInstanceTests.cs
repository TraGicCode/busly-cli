using BuslyCLI.Console.Tests.TestHelpers;
using Spectre.Console.Testing;

namespace BuslyCLI.Console.Tests.Commands.ServiceControl.Instance;

public class SetServiceControlInstanceTests : CommandTestBase
{
    [Test]
    public void ShouldOutputAMessageWhenInstanceDoesNotExist()
    {
        var yamlFile = """
                       ---
                       current-service-control-instance: local-sc
                       service-control-instances:
                         - name: local-sc
                           url: http://localhost:33333
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);
        var nonExistingInstance = $"{Guid.NewGuid():N}";

        var result = Sut.Run("servicecontrol", "instance", "set", nonExistingInstance, "--config", configFile.FilePath);

        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"""
             No service control instance exists with the name {nonExistingInstance}.
             """.NormalizeLineEndings()
        ));
    }

    [Test]
    public void ShouldOutputAMessageWhenInstanceIsSet()
    {
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

        var result = Sut.Run("servicecontrol", "instance", "set", "staging-sc", "--config", configFile.FilePath);

        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"""
             Switched to service control instance "staging-sc".
             """.NormalizeLineEndings()
        ));
    }

    [Test]
    public void ShouldBeIdempotentWhenSettingInstanceToTheAlreadyConfiguredInstance()
    {
        var yamlFile = """
                       ---
                       current-service-control-instance: local-sc
                       service-control-instances:
                         - name: local-sc
                           url: http://localhost:33333
                       """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        var result = Sut.Run("servicecontrol", "instance", "set", "local-sc", "--config", configFile.FilePath);

        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            $"""
             Switched to service control instance "local-sc".
             """.NormalizeLineEndings()
        ));
    }
}