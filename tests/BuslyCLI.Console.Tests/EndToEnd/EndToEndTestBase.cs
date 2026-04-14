using System.Text;
using System.Text.Json;
using BuslyCLI.Console.Tests.Commands;
using BuslyCLI.Console.Tests.TestHelpers;
using BuslyCLI.Infrastructure.Endpoints;
using BuslyCLI.Infrastructure.Factories;
using NServiceBus.Transport;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd;

public abstract class EndToEndTestBase : CommandTestBase
{
    protected readonly JsonSerializerOptions _jsonObjectOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    protected RawEndpoint TestEndpoint;

    protected abstract TransportConfig CreateTransportConfig();

    [SetUp]
    public async Task SetupEndpoint()
    {
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