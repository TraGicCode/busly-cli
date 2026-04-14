using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.NsbTimeout;

public class SendTimeoutCommandSettings : CommonCommandSettings
{
    [CommandOption("--do-not-deliver-before <do-not-deliver-before>")]
    [Description("Allows specifying a date before which the delivery should not occur, using ISO-8601 format (YYYY-MM-DDTHH:mm:ssZ)")]
    public DateTime? DoNotDeliverBefore { get; init; }

    [CommandOption("--delay-delivery-with <delay-delivery-with>")]
    //  ([days.]hh:mm:ss[.fffffff])
    [Description("Specifies the delay before the timeout is delivered, using a TimeSpan format")]
    public TimeSpan? DelayDeliveryWith { get; init; }

    public override ValidationResult Validate()
    {
        var baseResult = base.Validate();
        if (baseResult.Successful == false) return baseResult;
        // Neither provided
        if (DelayDeliveryWith is null && DoNotDeliverBefore is null)
        {
            return ValidationResult.Error(
                "You must specify either --do-not-deliver-before or --delay-delivery-with.");
        }

        // Both provided
        if (DelayDeliveryWith is not null && DoNotDeliverBefore is not null)
        {
            return ValidationResult.Error(
                "--do-not-deliver-before and --delay-delivery-with cannot be used together.");
        }

        return ValidationResult.Success();
    }
}