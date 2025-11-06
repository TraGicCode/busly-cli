using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using BuslyCLI.Config;
using BuslyCLI.Console.Tests.EndToEnd.Infrastructure;
using BuslyCLI.Console.Tests.TestHelpers;
using BuslyCLI.DependencyInjection;
using BuslyCLI.Spectre;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console.Testing;

namespace BuslyCLI.Console.Tests.EndToEnd.Learning;

[TestFixture]
public class SendCommandEndToEndLearningTests
{
    [SetUp]
    public void Setup()
    {
        var registrations = new ServiceCollection();
        registrations.AddBuslyCLIServices();
        registrations.AddYamlDeserializer();
        registrations.AddYamlSerializer();
        registrations.AddSingleton<INServiceBusConfiguration, NServiceBusConfiguration>();
        using var registrar = new DependencyInjectionRegistrar(registrations);
        _sut = new CommandAppTester(registrar);
        _sut.Configure(AppConfiguration.GetSpectreCommandConfiguration());
    }

    private CommandAppTester _sut;

    private readonly JsonSerializerOptions _jsonObjectOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    [Test]
    public async Task ShouldSendCommand()
    {
        await RunWithTestEndpoint(async testEndpoint =>
        {
            // Arrange
            await testEndpoint.StartEndpoint();
            var messageBody = new { OrderNumber = Guid.NewGuid() };
            var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
            var yamlFile = $"""
                            ---
                            current-transport: local-learning
                            transports:
                              - name: local-learning
                                learning-transport-config:
                                  storage-directory: ./.learningtransport
                            """;
            using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

            // Act
            var result = _sut.Run(
                "command",
                "send",
                "--content-type", "application/json",
                "--enclosed-message-type", "MessageContracts.Commands.CreateOrder",
                "--destination-endpoint", testEndpoint.EndpointName,
                "--message-body", json,
                "--config", configFile.FilePath);

            // Assert
            Assert.That(result.ExitCode, Is.EqualTo(0));
            var message = testEndpoint.TryReceiveMessage();
            Assert.That(message.Headers["NServiceBus.EnclosedMessageTypes"],
                Is.EqualTo("MessageContracts.Commands.CreateOrder"));
            Assert.That(message.Headers["NServiceBus.ContentType"], Is.EqualTo("application/json"));
            Assert.That(Encoding.UTF8.GetString(message.Body.Span), Is.EqualTo(json));
        });
    }

    [Test]
    public async Task ShouldPublishEvent()
    {
        await RunWithTestEndpoint(async testEndpoint =>
        {
            // Arrange
            await testEndpoint.StartEndpoint();
            await testEndpoint.Subscribe("MessageContracts.Events.OrderCreated");
            var messageBody = new { OrderNumber = Guid.NewGuid() };
            var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
            var yamlFile = $"""
                            ---
                            current-transport: local-learning
                            transports:
                              - name: local-learning
                                learning-transport-config:
                                  storage-directory: ./.learningtransport
                            """;
            using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

            // Act
            var result = _sut.Run(
                "event",
                "publish",
                "--content-type", "application/json",
                "--enclosed-message-type", "MessageContracts.Events.OrderCreated",
                "--message-body", json,
                "--config", configFile.FilePath);

            // Assert
            Assert.That(result.ExitCode, Is.EqualTo(0));
            var message = testEndpoint.TryReceiveMessage();
            Assert.That(message.Headers["NServiceBus.EnclosedMessageTypes"],
                Is.EqualTo("MessageContracts.Events.OrderCreated"));
            Assert.That(message.Headers["NServiceBus.ContentType"], Is.EqualTo("application/json"));
            Assert.That(Encoding.UTF8.GetString(message.Body.Span), Is.EqualTo(json));
        });
    }

    private async Task RunWithTestEndpoint(Func<TestEndpoint, Task> testAction)
    {
        var testEndpoint = await new TestEndpointFactory().CreateLearningTestEndpoint("./.learningtransport");

        try
        {
            await testAction(testEndpoint);
        }
        finally
        {
            await testEndpoint.ShutDownAndCleanUp();
        }
    }
}