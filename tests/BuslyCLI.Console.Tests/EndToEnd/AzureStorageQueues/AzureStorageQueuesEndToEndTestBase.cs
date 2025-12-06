using Testcontainers.Azurite;

namespace BuslyCLI.Console.Tests.EndToEnd.AzureStorageQueues;

[TestFixture]
public abstract class AzureStorageQueuesEndToEndTestBase : SingletonTestFixtureBase<AzuriteContainer>
{
    protected AzuriteContainer AzuriteContainer => Container;

    protected override AzuriteContainer CreateContainer()
    {
        return new AzuriteBuilder()
            .Build();
    }

    protected override async Task StartContainerAsync(AzuriteContainer container)
    {
        await container.StartAsync();
    }
}