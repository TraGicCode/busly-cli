using BuslyCLI.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Instance;

public class SetServiceControlInstanceCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration)
    : AsyncCommand<SetServiceControlInstanceSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, SetServiceControlInstanceSettings settings, CancellationToken cancellationToken)
    {
        var config = await nservicebusConfiguration.GetUnValidatedConfigurationAsync(settings.Config.Path);
        var targetInstance = settings.InstanceName.ToLower();

        if (config?.ServiceControlInstances != null &&
            config.ServiceControlInstances.Select(x => x.Name.ToLower()).Contains(targetInstance))
        {
            await nservicebusConfiguration.UpdateCurrentServiceControlInstanceAsync(settings.Config.Path, targetInstance);
            console.WriteLine($"Switched to service control instance \"{targetInstance}\".");
        }
        else
        {
            console.WriteLine($"No service control instance exists with the name {targetInstance}.");
        }

        return 0;
    }
}