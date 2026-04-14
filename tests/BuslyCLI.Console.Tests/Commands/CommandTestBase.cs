using BuslyCLI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console.Cli.Testing;

namespace BuslyCLI.Console.Tests.Commands;

public abstract class CommandTestBase
{
    protected CommandAppTester Sut { get; private set; } = null!;

    [SetUp]
    public void Setup()
    {
        var registrations = new ServiceCollection();
        registrations.AddBuslyCLIServices();
        using var registrar = new DependencyInjectionRegistrar(registrations);
        Sut = new CommandAppTester(registrar);
        Sut.Configure(AppConfiguration.GetSpectreCommandConfiguration());
    }
}
