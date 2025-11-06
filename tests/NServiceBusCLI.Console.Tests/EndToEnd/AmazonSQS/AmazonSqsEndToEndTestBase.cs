using Testcontainers.LocalStack;

namespace NServiceBusCLI.Console.Tests.EndToEnd.AmazonSQS;

[TestFixture]
public abstract class AmazonSqsEndToEndTestBase : SingletonTestFixtureBase<LocalStackContainer>
{
    protected LocalStackContainer LocalStackContainer => Container;

    protected override LocalStackContainer CreateContainer()
    {
        return new LocalStackBuilder()
            .WithImage("localstack/localstack:4")
            .WithEnvironment("SERVICES", "sqs,sns")
            .Build();
    }

    protected override async Task StartContainerAsync(LocalStackContainer container)
    {
        await container.StartAsync();
    }
}