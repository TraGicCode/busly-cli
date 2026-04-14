using System.Text;
using System.Text.Json;
using BuslyCLI.Config;
using BuslyCLI.DependencyInjection;
using BuslyCLI.Factories;
using BuslyCLI.Spectre;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Transport;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console.Cli.Testing;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd;

public abstract class EndToEndTestBase
{
    protected CommandAppTester _sut;

    protected readonly JsonSerializerOptions _jsonObjectOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    protected RawEndpoint TestEndpoint;

    protected abstract TransportConfig CreateTransportConfig();

    [SetUp]
    public async Task Setup()
    {
        var registrations = new ServiceCollection();
        registrations.AddBuslyCLIServices();
        using var registrar = new DependencyInjectionRegistrar(registrations);
        _sut = new CommandAppTester(registrar);
        _sut.Configure(AppConfiguration.GetSpectreCommandConfiguration());
        TestEndpoint = await new RawEndpointFactory()
            .CreateRawEndpoint(TestEndpointNameGenerator.GenerateUniqueEndpointName(), CreateTransportConfig());
        await TestEndpoint.StartEndpoint();
    }

    [TearDown]
    public async Task TearDown()
    {
        await TestEndpoint.ShutDownAndCleanUp();
    }

    protected static void AssertMessageReceived(IncomingMessage message, string enclosedMessageType, string expectedBody)
    {
        Assert.That(message.Headers["NServiceBus.EnclosedMessageTypes"], Is.EqualTo(enclosedMessageType));
        Assert.That(message.Headers["NServiceBus.ContentType"], Is.EqualTo("application/json"));
        Assert.That(Encoding.UTF8.GetString(message.Body.Span), Is.EqualTo(expectedBody));
    }
}