using BuslyCLI.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.Transport;

public class CurrentTransportCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration)
    : AsyncCommand<CurrentTransportSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CurrentTransportSettings settings, CancellationToken cancellationToken)
    {
        var nsbConfiguration = await nservicebusConfiguration.GetConfigurationAsync(settings.Config.Path);

        console.WriteLine(nsbConfiguration != null && nsbConfiguration.CurrentTransport is not null ? nsbConfiguration.CurrentTransport : "Current transport is not set.");
        return 0;
    }
}