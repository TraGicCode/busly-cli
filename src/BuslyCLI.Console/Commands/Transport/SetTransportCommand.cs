using BuslyCLI.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.Transport;

public class SetTransportCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration)
    : AsyncCommand<SetTransportSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, SetTransportSettings settings, CancellationToken cancellationToken)
    {
        var nsbConfiguration = await nservicebusConfiguration.GetValidatedConfigurationAsync(settings.Config.Path);
        var targetTransport = settings.TransportName.ToLower();
        if (nsbConfiguration.Transports.Select(x => x.Name.ToLower()).Contains(targetTransport))
        {
            await nservicebusConfiguration.UpdateCurrentTransportAsync(settings.Config.Path, targetTransport);
            console.WriteLine($"Switched to transport \"{targetTransport}\".");
        }
        else
        {
            console.WriteLine($"No transport exists with the name {targetTransport}.");
        }

        return 0;
    }
}