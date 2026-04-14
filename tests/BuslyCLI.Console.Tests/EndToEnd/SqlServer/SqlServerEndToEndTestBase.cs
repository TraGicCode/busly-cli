using BuslyCLI.Config;
using Testcontainers.MsSql;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd.SqlServer;

public abstract class SqlServerEndToEndTestBase : SingletonTestFixtureBase<MsSqlContainer>
{
    protected override TransportConfig CreateTransportConfig() => new()
    {
        SqlServerTransportConfig = new SqlServerTransportConfig
        {
            ConnectionString = Container.GetConnectionString()
        }
    };

    protected override MsSqlContainer CreateContainer()
    {
        return new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
            .Build();
    }

    protected override async Task StartContainerAsync(MsSqlContainer container)
    {
        await container.StartAsync();
    }
}