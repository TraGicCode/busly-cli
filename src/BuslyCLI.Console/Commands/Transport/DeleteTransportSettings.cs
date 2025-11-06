using Spectre.Console.Cli;

namespace BuslyCLI.Commands.Transport;

public class DeleteTransportSettings : GlobalCommandSettings
{
    [CommandArgument(0, "<name>")]
    public string TransportName { get; set; }
}