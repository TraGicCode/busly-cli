using BuslyCLI.Config;
using BuslyCLI.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.CustomCheck;

public class ListCustomChecksCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration, ServiceControlClient serviceControlClient)
    : AsyncCommand<ListCustomChecksSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, ListCustomChecksSettings settings, CancellationToken cancellationToken)
    {
        var config = await nservicebusConfiguration.GetServiceControlValidatedConfigurationAsync(settings.Config.Path);

        var customChecks = await serviceControlClient.GetCustomChecksAsync(config.CurrentServiceControlInstanceConfig.Url,
            status: "fail",
            page: settings.Page,
            pageSize: settings.PageSize,
            cancellationToken);

        var table = new Table();
        table.AddColumn("Reported At");
        table.AddColumn("Status");
        table.AddColumn("Category");
        table.AddColumn("Reason");
        table.AddColumn("Endpoint");

        foreach (var customCheck in customChecks.OrderByDescending(x => x.ReportedAt))
        {
            table.AddRow(
                customCheck.ReportedAt.ToString("u"),
                // TODO: Currently it's hardcoded to only show failures in ServiceControlClient
                customCheck.Status == "fail" ? "[red]Fail[/]" : "[green]Pass[/]",
                customCheck.Category,
                customCheck.FailureReason,
                $"{customCheck.OriginatingEndpoint.Name} ({customCheck.OriginatingEndpoint.Host})");
        }

        console.Write(table);

        return 0;
    }
}