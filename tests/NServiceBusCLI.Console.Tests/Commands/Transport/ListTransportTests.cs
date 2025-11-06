using Microsoft.Extensions.DependencyInjection;
using NServiceBusCLI.Config;
using NServiceBusCLI.Console.Tests.TestHelpers;
using NServiceBusCLI.DependencyInjection;
using NServiceBusCLI.Spectre;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console.Testing;

namespace NServiceBusCLI.Console.Tests.Commands.Transport;

public class ListTransportTests
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
    public void ShouldOutputAnEmptyGrid()
    {
        // Arrange
        var yamlFile = "---";
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);
        var result = _sut.Run("transport", "list", "--config", configFile.FilePath);

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
        var result = _sut.Run("transport", "list", "--config", configFile.FilePath);

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