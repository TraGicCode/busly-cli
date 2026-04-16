using BuslyCLI.Config.Transports;
using Testcontainers.ServiceBus;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd.AzureServiceBus;

public abstract class AzureServiceBusEndToEndTestBase : SingletonTestFixtureBase<ServiceBusContainer>
{


    protected override TransportConfig CreateTransportConfig() => new()
    {
        AzureServiceBusTransportConfig = new AzureServiceBusTransportConfig
        {
            ConnectionString = Container.GetConnectionString()
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