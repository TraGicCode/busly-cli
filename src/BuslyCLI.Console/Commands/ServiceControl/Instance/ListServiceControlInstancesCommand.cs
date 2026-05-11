using BuslyCLI.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Instance;

public class ListServiceControlInstancesCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration)
    : AsyncCommand<ListServiceControlInstancesSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, ListServiceControlInstancesSettings settings, CancellationToken cancellationToken)
    {
        var grid = new Grid();
        // Add columns
        grid.AddColumn();
        grid.AddColumn();
        grid.AddColumn();
        // Add header row
        grid.AddRow("CURRENT", "NAME", "URL");

        var config = await nservicebusConfiguration.GetUnValidatedConfigurationAsync(settings.Config.Path);

        if (config is { ServiceControlInstances: not null })
        {
            foreach (var instance in config.ServiceControlInstances)
            {
                grid.AddRow(
                    config.CurrentServiceControlInstance == instance.Name ? "*" : "",
                    instance.Name,
                    instance.Url);
            }
        }

        console.Write(grid);
        return 0;
    }
}