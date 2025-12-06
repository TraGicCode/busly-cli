using Testcontainers.PostgreSql;

namespace BuslyCLI.Console.Tests.EndToEnd.PostgreSql;

[TestFixture]
public abstract class PostgreSqlEndToEndTestBase : SingletonTestFixtureBase<PostgreSqlContainer>
{
    protected PostgreSqlContainer PostgreSqlContainer => Container;

    protected override PostgreSqlContainer CreateContainer()
    {
        return new PostgreSqlBuilder()
            .Build();
    }

    protected override async Task StartContainerAsync(PostgreSqlContainer container)
    {
        await container.StartAsync();
    }
}