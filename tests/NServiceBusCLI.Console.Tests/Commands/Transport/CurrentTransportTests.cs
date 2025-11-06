using Microsoft.Extensions.DependencyInjection;
using NServiceBusCLI.Config;
using NServiceBusCLI.Console.Tests.TestHelpers;
using NServiceBusCLI.DependencyInjection;
using NServiceBusCLI.Spectre;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console.Testing;

namespace NServiceBusCLI.Console.Tests.Commands.Transport;

public class CurrentTransportTests
{
    private CommandAppTester _sut;

    [SetUp]
    public void Setup()
    {
        var registrations = new ServiceCollection();
        registrations.AddNServiceBusCliServices();
        registrations.AddYamlDeserializer();
        registrations.AddYamlSerializer();
        registrations.AddSingleton<INServiceBusConfiguration, NServiceBusConfiguration>();
        using var registrar = new DependencyInjectionRegistrar(registrations);
        _sut = new CommandAppTester(registrar);
        _sut.Configure(AppConfiguration.GetSpectreCommandConfiguration());
    }

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
        var result = _sut.Run("transport", "current", "--config", configFile.FilePath);

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
        var result = _sut.Run("transport", "current", "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            """
                Current transport is not set.
                """.NormalizeLineEndings()
        ));
    }
}