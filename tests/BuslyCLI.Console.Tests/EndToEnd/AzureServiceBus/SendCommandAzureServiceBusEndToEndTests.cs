using System.Text;
using System.Text.Json;
using BuslyCLI.Config;
using BuslyCLI.Console.Tests.EndToEnd.Infrastructure;
using BuslyCLI.Console.Tests.TestHelpers;
using BuslyCLI.DependencyInjection;
using BuslyCLI.Spectre;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console.Testing;

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
        registrations.AddYamlDeserializer();
        registrations.AddYamlSerializer();
        registrations.AddSingleton<INServiceBusConfiguration, NServiceBusConfiguration>();
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
    // TODO: Remove endpoint.Subscribe("<<EVENT>>") calls
    // TODO: in `HostSettings` pass `false` for the setupInfrastructure parameter
    // TODO: [Option 1 - NOT EASY] Find a way so that i can "pre-determine" the number of test methods
    //       to pregenerate a list of queues, subscriptions, and topics
    // TODO: [Option 2 - precreate a list of events and random endpoint names. pull the names from the list during execution so it can't be used by another test]
    // TODO: [Option 3 - create and destroy a container per test.  This would be slow and possibly a resource hog if done in paralle]
    public async Task ShouldPublishEvent()
    {
        await RunWithTestEndpoint(async testEndpoint =>
        {
            // Arrange
            await testEndpoint.StartEndpoint();
            // This wont work.  Emulator doesn't allow subscription creation.  It's all pre-setup with emulator config
            // await testEndpoint.Subscribe("MessageContracts.Events.OrderCreated");
            var messageBody = new { OrderNumber = Guid.NewGuid() };

            var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
            var eventType = GeneratedTestEndpointNamesAndSubscribedEvent
                .Single(x => x.Item1 == testEndpoint.EndpointName).Item2;

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

    private async Task RunWithTestEndpoint(Func<TestEndpoint, Task> testAction)
    {
        var random = new Random();
        var testEndpointName = GeneratedTestEndpointNamesAndSubscribedEvent[random.Next(GeneratedTestEndpointNamesAndSubscribedEvent.Count)];
        var testEndpoint = await new TestEndpointFactory().CreateAzureServiceBusTestEndpoint(testEndpointName.Item1, Container.GetConnectionString());

        await testAction(testEndpoint);
        await testEndpoint.ShutDownAndCleanUp();
    }
}