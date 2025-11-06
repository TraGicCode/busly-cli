using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands;

public abstract class CommonCommandSettings : CommonMessageSettings
{
    [CommandOption("-d|--destination-endpoint <destination-endpoint>")]
    [Description("The destination endpoint to send a message to")]
    public required string DestinationEndpoint { get; set; }


    public override ValidationResult Validate()
    {
        var baseResult = base.Validate();
        if (baseResult.Successful == false) return baseResult;

        if (string.IsNullOrWhiteSpace(DestinationEndpoint))
            return ValidationResult.Error("must specify a 'destination-endpoint'.");
        return ValidationResult.Success();
    }
}