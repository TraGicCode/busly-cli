using BuslyCLI.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.Transport;

public class DeleteTransportCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration)
    : AsyncCommand<DeleteTransportSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DeleteTransportSettings settings, CancellationToken cancellationToken)
    {
        var nsbConfiguration = await nservicebusConfiguration.GetConfigurationAsync(settings.Config.Path);
        var targetTransport = settings.TransportName.ToLower();
        if (nsbConfiguration.Transports.Select(x => x.Name.ToLower()).Contains(targetTransport))
        {
            if (nsbConfiguration.CurrentTransport.ToLower() == targetTransport)
            {
                nsbConfiguration.CurrentTransport = "";
                console.WriteLine("This removed your active transport, use \"nservicebus transport set\" to select a different one.");
            }
            nsbConfiguration.Transports = nsbConfiguration.Transports.Where(x => x.Name.ToLower() != targetTransport).ToList();
            console.WriteLine($"deleted transport named {targetTransport} from {settings.Config.Path}");
            await nservicebusConfiguration.PersistConfiguration(settings.Config.Path, nsbConfiguration);
        }
        else
        {
            console.WriteLine($"Cannot delete transport {settings.TransportName} since it doesn't exist in the config file.");
        }

        return 0;
    }
}