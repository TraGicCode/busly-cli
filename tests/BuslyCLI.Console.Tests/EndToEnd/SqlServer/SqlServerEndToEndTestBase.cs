using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace BuslyCLI.Console.Tests.EndToEnd.SqlServer;

[TestFixture]
public abstract class SqlServerEndToEndTestBase : SingletonTestFixtureBase<MsSqlContainer>
{
    protected MsSqlContainer SqlServerContainer => Container;

    protected override MsSqlContainer CreateContainer()
    {
        return new MsSqlBuilder()
            .Build();
    }

    protected override async Task StartContainerAsync(MsSqlContainer container)
    {
        await container.StartAsync();
    }
}