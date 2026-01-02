using Testcontainers.RabbitMq;

namespace BuslyCLI.Console.Tests.EndToEnd.RabbitMQ;

[TestFixture]
public abstract class RabbitMqEndToEndTestBase : SingletonTestFixtureBase<RabbitMqContainer>
{
    protected RabbitMqContainer RabbitMqContainer => Container;

    protected override RabbitMqContainer CreateContainer()
    {
        return new RabbitMqBuilder("rabbitmq:3-management")
            .WithPortBinding(15672, true) // Bind host port 15673 to container port 15672 (Management UI)
            .Build();
    }

    protected override async Task StartContainerAsync(RabbitMqContainer container)
    {
        await container.StartAsync();
    }
}