using BuslyCLI.Config;
using BuslyCLI.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Endpoint;

public class ListEndpointsCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration, ServiceControlClient serviceControlClient)
    : AsyncCommand<ListEndpointsSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, ListEndpointsSettings settings, CancellationToken cancellationToken)
    {
        var config = await nservicebusConfiguration.GetServiceControlValidatedConfigurationAsync(settings.Config.Path);

        var endpoints = await serviceControlClient.GetEndpointsAsync(config.CurrentServiceControlInstanceConfig.Url, cancellationToken);

        var table = new Table();
        table.AddColumn("Id");
        table.AddColumn("Name");
        table.AddColumn("Host Display Name");
        table.AddColumn("Sending Heartbeats");
        table.AddColumn("Heartbeat Status");
        table.AddColumn("Last Reported At");

        foreach (var endpoint in endpoints.OrderByDescending(x => x.Name))
        {
            table.AddRow(
                endpoint.Id,
                endpoint.Name,
                endpoint.HostDisplayName,
                endpoint.IsSendingHeartbeats ? "Yes" : "No",
                endpoint.HeartbeatInformation.ReportedStatus == "dead" ? "[red]Dead[/]" : "[green]Alive[/]",
                endpoint.HeartbeatInformation.LastReportAt.ToString("u"));
        }

        console.Write(table);

        return 0;
    }
}