using BuslyCLI.Config;
using BuslyCLI.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Message;

// http://localhost:9091/api/messages2/?endpoint_name=&from=&to=&q=ttttt&page_size=100&sort=time_sent&direction=desc
public class SearchMessagesCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration, ServiceControlClient serviceControlClient)
    : AsyncCommand<SearchMessagesSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, SearchMessagesSettings settings, CancellationToken cancellationToken)
    {
        var config = await nservicebusConfiguration.GetServiceControlValidatedConfigurationAsync(settings.Config.Path);

        var messages = await serviceControlClient.SearchMessagesAsync(
            config.CurrentServiceControlInstanceConfig.Url,
            q: settings.Keyword,
            endpointName: settings.Endpoint,
            from: settings.From,
            to: settings.To,
            pageSize: settings.PageSize,
            sort: settings.Sort.GetDescription(),
            direction: settings.SortDirection.GetDescription(),
            cancellationToken: cancellationToken);

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Message Type");
        table.AddColumn("Processing Time");
        table.AddColumn("Critical Time");
        table.AddColumn("Delivery Time");
        table.AddColumn("Time Sent");

        foreach (var message in messages.OrderByDescending(x => x.TimeSent))
        {
            table.AddRow(
                message.Id,
                message.MessageType,
                message.ProcessingTime,
                message.CriticalTime.StartsWith("-") ? "00:00:00" : message.ProcessingTime,
                message.DeliveryTime.StartsWith("-") ? message.DeliveryTime.Substring(1) : message.DeliveryTime,
                message.TimeSent.Value.ToString("u"));
        }

        console.Write(table);

        return 0;
    }
}