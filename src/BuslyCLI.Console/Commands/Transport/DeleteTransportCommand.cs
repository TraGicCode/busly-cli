using BuslyCLI.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.Transport;

public class DeleteTransportCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration)
    : AsyncCommand<DeleteTransportSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, DeleteTransportSettings settings, CancellationToken cancellationToken)
    {
        var nsbConfiguration = await nservicebusConfiguration.GetValidatedConfigurationAsync(settings.Config.Path);
        var targetTransport = settings.TransportName.ToLower();
        if (nsbConfiguration.Transports.Select(x => x.Name.ToLower()).Contains(targetTransport))
        {
            if (nsbConfiguration.CurrentTransport.ToLower() == targetTransport)
            {
                await nservicebusConfiguration.UpdateCurrentTransportAsync(settings.Config.Path, "");
                console.WriteLine("This removed your active transport, use \"busly transport set\" to select a different one.");
            }
            await nservicebusConfiguration.RemoveTransportAsync(settings.Config.Path, targetTransport);
            console.WriteLine($"deleted transport named {targetTransport} from {settings.Config.Path}");
        }
        else
        {
            console.WriteLine($"Cannot delete transport {settings.TransportName} since it doesn't exist in the config file.");
        }

        return 0;
    }
}