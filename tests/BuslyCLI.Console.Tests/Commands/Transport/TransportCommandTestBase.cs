using BuslyCLI.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console.Cli.Testing;

namespace BuslyCLI.Console.Tests.Commands.Transport;

public abstract class TransportCommandTestBase
{
    protected CommandAppTester Sut;

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
