using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NServiceBusCLI.Config;
using NServiceBusCLI.DependencyInjection;
using NServiceBusCLI.Spectre;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

if (args.Contains("--attach", StringComparer.OrdinalIgnoreCase))
{
    Console.WriteLine($"Waiting for debugger attach. PID: {Environment.ProcessId}");
    while (!Debugger.IsAttached) await Task.Delay(1000);
}

var registrations = new ServiceCollection();
registrations.AddNServiceBusCliServices();
registrations.AddYamlDeserializer();
registrations.AddYamlSerializer();
registrations.AddSingleton<INServiceBusConfiguration, NServiceBusConfiguration>();
using var registrar = new DependencyInjectionRegistrar(registrations);
var app = new CommandApp(registrar);
app.Configure(AppConfiguration.GetSpectreCommandConfiguration());

return await app.RunAsync(args);