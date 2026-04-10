using System.Text;
using System.Text.Json;
using BuslyCLI.Config;
using BuslyCLI.Console.Tests.TestHelpers;
using BuslyCLI.DependencyInjection;
using BuslyCLI.Factories;
using BuslyCLI.Spectre;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console.Cli.Testing;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd.AzureServiceBus;

// [Ignore("Can enable once AzureServiceBus is added to the nsb config file")]
public class SendCommandAzureServiceBusEndToEndTests : AzureServiceBusEndToEndTestBase
{
    private CommandAppTester _sut;
    private readonly JsonSerializerOptions _jsonObjectOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    [SetUp]
    public void Setup()
    {
        var registrations = new ServiceCollection();
        registrations.AddBuslyCLIServices();
        using var registrar = new DependencyInjectionRegistrar(registrations);
        _sut = new CommandAppTester(registrar);
        _sut.Configure(AppConfiguration.GetSpectreCommandConfiguration());
    }

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
                            current-transport: local-azure-service-bus
                            transports:
                              - name: local-azure-service-bus
                                azure-service-bus-transport-config:
                                  connection-string: {Container.GetConnectionString()}
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
            var eventType = $"MessageContracts.Events.OrderCreated-{Guid.NewGuid():N}";
            await testEndpoint.Subscribe(eventType);
            var messageBody = new { OrderNumber = Guid.NewGuid() };

            var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);

            var yamlFile = $"""
                            ---
                            current-transport: local-azure-service-bus
                            transports:
                              - name: local-azure-service-bus
                                azure-service-bus-transport-config:
                                  connection-string: {Container.GetConnectionString()}
                            """;
            using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

            // Act
            var result = _sut.Run(
                "event",
                "publish",
                "--content-type", "application/json",
                "--enclosed-message-type", eventType,
                "--message-body", json,
                "--config", configFile.FilePath);

            // Assert
            Assert.That(result.ExitCode, Is.EqualTo(0));
            var message = testEndpoint.TryReceiveMessage();
            Assert.That(message.Headers["NServiceBus.EnclosedMessageTypes"],
                Is.EqualTo(eventType));
            Assert.That(message.Headers["NServiceBus.ContentType"], Is.EqualTo("application/json"));
            Assert.That(Encoding.UTF8.GetString(message.Body.Span), Is.EqualTo(json));
        });
    }

    private async Task RunWithTestEndpoint(Func<RawEndpoint, Task> testAction)
    {
        var testEndpointName = $"TestEndpoint-{Guid.NewGuid():N}";
        var testEndpoint = await new RawEndpointFactory().CreateRawEndpoint(testEndpointName, new TransportConfig()
        {
            AzureServiceBusTransportConfig = new AzureServiceBusTransportConfig()
            {
                ConnectionString = GetNServiceBusConnectionString()
            }
        }, true);

        await testAction(testEndpoint);
        await testEndpoint.ShutDownAndCleanUp();
    }
}