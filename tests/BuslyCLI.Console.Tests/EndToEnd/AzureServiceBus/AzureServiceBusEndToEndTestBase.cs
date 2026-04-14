using BuslyCLI.Config;
using Testcontainers.ServiceBus;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd.AzureServiceBus;

public abstract class AzureServiceBusEndToEndTestBase : SingletonTestFixtureBase<ServiceBusContainer>
{
    // GetConnectionString() uses UriBuilder which appends a trailing slash to the endpoint
    // (e.g. "sb://localhost:12345/"). NServiceBus's InjectEmulatorAdminPort does a string replace
    // looking for "Endpoint=sb://localhost:12345;" (no trailing slash), so it never matches and
    // the admin client ends up using the AMQP port instead of port 5300.
    // This method builds the connection string without the trailing slash so port injection works correctly.
    protected string GetNServiceBusConnectionString()
    {
        var amqpPort = Container.GetMappedPublicPort(ServiceBusBuilder.ServiceBusPort);
        return $"Endpoint=sb://{Container.Hostname}:{amqpPort};SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
    }

    protected override TransportConfig CreateTransportConfig() => new()
    {
        AzureServiceBusTransportConfig = new AzureServiceBusTransportConfig
        {
            ConnectionString = GetNServiceBusConnectionString()
        }
    };

    protected override ServiceBusContainer CreateContainer()
    {
        return new ServiceBusBuilder("mcr.microsoft.com/azure-messaging/servicebus-emulator:latest")
            .WithAcceptLicenseAgreement(true)
            .WithPortBinding(ServiceBusBuilder.ServiceBusHttpPort, ServiceBusBuilder.ServiceBusHttpPort)
            .Build();
    }

    protected override async Task StartContainerAsync(ServiceBusContainer container)
    {
        await container.StartAsync();
    }
}