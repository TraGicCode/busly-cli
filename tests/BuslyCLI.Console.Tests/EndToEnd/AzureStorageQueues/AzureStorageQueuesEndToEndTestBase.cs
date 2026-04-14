using BuslyCLI.Config.Transports;
using Testcontainers.Azurite;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd.AzureStorageQueues;

public abstract class AzureStorageQueuesEndToEndTestBase : SingletonTestFixtureBase<AzuriteContainer>
{
    protected override TransportConfig CreateTransportConfig() => new()
    {
        AzureStorageQueuesTransportConfig = new AzureStorageQueuesTransportConfig
        {
            ConnectionString = Container.GetConnectionString()
        }
    };

    protected override AzuriteContainer CreateContainer()
    {
        return new AzuriteBuilder("mcr.microsoft.com/azure-storage/azurite")
            .WithCommand("--skipApiVersionCheck")
            .Build();
    }

    protected override async Task StartContainerAsync(AzuriteContainer container)
    {
        await container.StartAsync();
    }
}