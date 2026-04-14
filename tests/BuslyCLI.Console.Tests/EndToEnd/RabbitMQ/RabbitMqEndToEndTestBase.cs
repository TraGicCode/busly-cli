using BuslyCLI.Config;
using Testcontainers.RabbitMq;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd.RabbitMQ;

public abstract class RabbitMqEndToEndTestBase : SingletonTestFixtureBase<RabbitMqContainer>
{
    protected override TransportConfig CreateTransportConfig() => new()
    {
        RabbitmqTransportConfig = new RabbitmqTransportConfig
        {
            AmqpConnectionString = Container.GetConnectionString(),
            ManagementApi = new ManagementApi
            {
                Url = $"http://{Container.Hostname}:{Container.GetMappedPublicPort(15672)}"
            }
        }
    };

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