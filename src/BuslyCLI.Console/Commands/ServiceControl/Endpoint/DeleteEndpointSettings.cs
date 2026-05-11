using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Endpoint;

public class DeleteEndpointSettings : GlobalCommandSettings
{
    [CommandArgument(0, "<id>")]
    public Guid Id { get; set; }
}