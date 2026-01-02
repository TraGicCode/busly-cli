using Testcontainers.MsSql;

namespace BuslyCLI.Console.Tests.EndToEnd.SqlServer;

[TestFixture]
public abstract class SqlServerEndToEndTestBase : SingletonTestFixtureBase<MsSqlContainer>
{
    protected MsSqlContainer SqlServerContainer => Container;

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