using BuslyCLI.Config;
using BuslyCLI.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Endpoint;

public class DeleteEndpointCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration, ServiceControlClient serviceControlClient)
    : AsyncCommand<DeleteEndpointSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, DeleteEndpointSettings settings, CancellationToken cancellationToken)
    {
        var config = await nservicebusConfiguration.GetServiceControlValidatedConfigurationAsync(settings.Config.Path);

        if (AnsiConsole.Confirm("Are you sure you want to delete this endpoint? This action cannot be undone and all data associated with this endpoint will be lost."))
        {
            var wasDeleted = await serviceControlClient.DeleteEndpointAsync(config.CurrentServiceControlInstanceConfig.Url, settings.Id, cancellationToken);
            if (!wasDeleted)
            {
                console.WriteLine($"Endpoint with id {settings.Id} did not exist at {config.CurrentServiceControlInstanceConfig.Name}.");
                return 1;
            }
            console.WriteLine($"Deleted endpoint with id {settings.Id} from {config.CurrentServiceControlInstanceConfig.Name}.");
        }

        return 0;
    }
}