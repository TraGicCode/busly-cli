using BuslyCLI.Commands.ServiceControl.Endpoint;
using BuslyCLI.Config;
using BuslyCLI.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

public class ShowLicenseCommand(
    IAnsiConsole console,
    INServiceBusConfiguration nservicebusConfiguration,
    ServiceControlClient serviceControlClient)
    : AsyncCommand<ListEndpointsSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, ListEndpointsSettings settings,
        CancellationToken cancellationToken)
    {
        var config = await nservicebusConfiguration.GetServiceControlValidatedConfigurationAsync(settings.Config.Path);

        var license =
            await serviceControlClient.GetLicenseAsync(config.CurrentServiceControlInstanceConfig.Url,
                cancellationToken);

        var table = new Table();
        table.AddColumn("Status");
        table.AddColumn("Registered To");
        table.AddColumn("Is Trial");
        table.AddColumn("Edition");
        table.AddColumn("Expiration Date");


        table.AddRow(
            license.Status == "valid" ? "[green]Valid[/]" : "[red]Invalid[/]",
            license.RegisteredTo,
            license.TrialLicense ? "Yes" : "No",
            license.Edition,
            license.ExpirationDate.HasValue ? license.ExpirationDate.Value.ToString("u") : "N/A");

        console.Write(table);

        return 0;
    }
}