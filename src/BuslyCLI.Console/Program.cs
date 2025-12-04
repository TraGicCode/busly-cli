using System.Diagnostics;
using BuslyCLI.DependencyInjection;
using BuslyCLI.Spectre;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

if (args.Contains("--attach", StringComparer.OrdinalIgnoreCase))
{
    Console.WriteLine($"Waiting for debugger attach. PID: {Environment.ProcessId}");
    while (!Debugger.IsAttached) await Task.Delay(1000);
}

var registrations = new ServiceCollection();
registrations.AddBuslyCLIServices();
using var registrar = new DependencyInjectionRegistrar(registrations);
var app = new CommandApp(registrar);
app.Configure(AppConfiguration.GetSpectreCommandConfiguration());

return await app.RunAsync(args);