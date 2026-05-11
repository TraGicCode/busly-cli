using BuslyCLI.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Instance;

public class DeleteServiceControlInstanceCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration)
    : AsyncCommand<DeleteServiceControlInstanceSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, DeleteServiceControlInstanceSettings settings, CancellationToken cancellationToken)
    {
        var config = await nservicebusConfiguration.GetUnValidatedConfigurationAsync(settings.Config.Path);
        var targetInstance = settings.InstanceName.ToLower();

        if (config?.ServiceControlInstances != null &&
            config.ServiceControlInstances.Select(x => x.Name.ToLower()).Contains(targetInstance))
        {
            if (config.CurrentServiceControlInstance?.ToLower() == targetInstance)
            {
                await nservicebusConfiguration.UpdateCurrentServiceControlInstanceAsync(settings.Config.Path, "");
                console.WriteLine("This removed your active service control instance, use \"busly servicecontrol instance set\" to select a different one.");
            }

            await nservicebusConfiguration.RemoveServiceControlInstanceAsync(settings.Config.Path, targetInstance);
            console.WriteLine($"Deleted service control instance named {targetInstance} from {settings.Config.Path}");
        }
        else
        {
            console.WriteLine($"Cannot delete service control instance {settings.InstanceName} since it doesn't exist in the config file.");
        }

        return 0;
    }
}