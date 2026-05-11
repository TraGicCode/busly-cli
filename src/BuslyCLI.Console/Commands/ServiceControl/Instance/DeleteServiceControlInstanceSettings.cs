using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Instance;

public class DeleteServiceControlInstanceSettings : GlobalCommandSettings
{
    [CommandArgument(0, "<name>")]
    public string InstanceName { get; set; }
}