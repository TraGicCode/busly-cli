using BuslyCLI.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Instance;

public class CurrentServiceControlInstanceCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration)
    : AsyncCommand<CurrentServiceControlInstanceSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CurrentServiceControlInstanceSettings settings, CancellationToken cancellationToken)
    {
        var config = await nservicebusConfiguration.GetUnValidatedConfigurationAsync(settings.Config.Path);

        console.WriteLine(config?.CurrentServiceControlInstance is not null
            ? config.CurrentServiceControlInstance
            : "Current service control instance is not set.");

        return 0;
    }
}