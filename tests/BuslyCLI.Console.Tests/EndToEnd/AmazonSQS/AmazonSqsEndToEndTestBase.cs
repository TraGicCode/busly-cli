using BuslyCLI.Config.Transports;
using Testcontainers.LocalStack;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd.AmazonSQS;

public abstract class AmazonSqsEndToEndTestBase : SingletonTestFixtureBase<LocalStackContainer>
{
    protected override TransportConfig CreateTransportConfig() => new()
    {
        AmazonsqsTransportConfig = new AmazonsqsTransportConfig
        {
            ServiceUrl = Container.GetConnectionString(),
            RegionName = "us-east-1",
            AccessKey = "test",
            SecretKey = "test"
        }
    };

    protected override LocalStackContainer CreateContainer()
    {
        return new LocalStackBuilder("localstack/localstack:4")
            .WithEnvironment("SERVICES", "sqs,sns")
            .Build();
    }

    protected override async Task StartContainerAsync(LocalStackContainer container)
    {
        await container.StartAsync();
    }
}