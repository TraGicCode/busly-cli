using BuslyCLI.Config;
using Testcontainers.PostgreSql;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd.PostgreSql;

public abstract class PostgreSqlEndToEndTestBase : SingletonTestFixtureBase<PostgreSqlContainer>
{
    protected override TransportConfig CreateTransportConfig() => new()
    {
        PostgreSqlTransportConfig = new PostgreSqlTransportConfig
        {
            ConnectionString = Container.GetConnectionString()
        }
    };

    protected override PostgreSqlContainer CreateContainer()
    {
        return new PostgreSqlBuilder("postgres:latest")
            .Build();
    }

    protected override async Task StartContainerAsync(PostgreSqlContainer container)
    {
        await container.StartAsync();
    }
}