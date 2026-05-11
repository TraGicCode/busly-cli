using BuslyCLI.Config;
using BuslyCLI.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Event;

public class ListEventsCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration, ServiceControlClient serviceControlClient)
    : AsyncCommand<ListEventsSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, ListEventsSettings settings, CancellationToken cancellationToken)
    {
        var config = await nservicebusConfiguration.GetServiceControlValidatedConfigurationAsync(settings.Config.Path);

        var eventLogItems = await serviceControlClient.GetEventLogItemsAsync(config.CurrentServiceControlInstanceConfig.Url, page: settings.PageNumber, perPage: settings.PageSize, cancellationToken);

        var table = new Table();
        table.AddColumn("Occured At");
        table.AddColumn("Severity");
        table.AddColumn("Description");

        foreach (var eventLogItem in eventLogItems.OrderByDescending(x => x.RaisedAt))
        {
            table.AddRow(
                eventLogItem.RaisedAt.ToString("u"),
                eventLogItem.Severity == "error" ? "[red]Error[/]" : "Info",
                eventLogItem.Description);
        }

        console.Write(table);

        return 0;
    }
}