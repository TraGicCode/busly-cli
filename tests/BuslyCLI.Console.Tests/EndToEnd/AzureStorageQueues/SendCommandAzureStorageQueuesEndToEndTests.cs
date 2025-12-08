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

namespace BuslyCLI.Console.Tests.EndToEnd.AzureStorageQueues;

[TestFixture]
public class SendCommandAzureStorageQueuesEndToEndTests : AzureStorageQueuesEndToEndTestBase
{
    [SetUp]
    public void Setup()
    {
        var registrations = new ServiceCollection();
        registrations.AddBuslyCLIServices();
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
                            current-transport: local-azure-storage-queues
                            transports:
                              - name: local-azure-storage-queues
                                azure-storage-queues-transport-config:
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
            await testEndpoint.Subscribe("MessageContracts.Events.OrderCreated");
            var messageBody = new { OrderNumber = Guid.NewGuid() };
            var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
            var yamlFile = $"""
                            ---
                            current-transport: local-azure-storage-queues
                            transports:
                              - name: local-azure-storage-queues
                                azure-storage-queues-transport-config:
                                  connection-string: {Container.GetConnectionString()}
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

    // Test Endpoint
    // Example of how to wait for and get messages
    // https://github.com/Particular/NServiceBus.RabbitMQ/blob/dba627a5a2c50519d7a2466efe3f76c8d5c8828d/src/NServiceBus.Transport.RabbitMQ.Tests/RabbitMqContext.cs#L41
    private async Task RunWithTestEndpoint(Func<RawEndpoint, Task> testAction)
    {
        var testEndpoint = await new RawEndpointFactory().CreateRawEndpoint(TestEndpointNameGenerator.GenerateUniqueEndpointName(), new TransportConfig()
        {
            AzureStorageQueuesTransportConfig = new AzureStorageQueuesTransportConfig()
            {
                ConnectionString = Container.GetConnectionString()
            }
        });

        await testAction(testEndpoint);
        await testEndpoint.ShutDownAndCleanUp();
    }
}