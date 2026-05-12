using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Message;

public class SearchMessagesSettings : GlobalCommandSettings
{
    [CommandArgument(0, "[keyword]")]
    public string Keyword { get; set; } = "";

    [CommandOption("--endpoint <endpoint>")]
    [Description("Filter to one endpoint")]
    public string Endpoint { get; set; }

    [CommandOption("--page-size <page-size>")]
    [DefaultValue(50)]
    [Description("The page size to use when searching messages")]
    public int PageSize { get; set; }

    [CommandOption("--from <from>")]
    [Description("The start date to search messages from, using ISO-8601 format (YYYY-MM-DDTHH:mm:ssZ)")]
    public DateTime? From { get; set; }

    [CommandOption("--to <to>")]
    [Description("The end date to search messages to, using ISO-8601 format (YYYY-MM-DDTHH:mm:ssZ)")]
    public DateTime? To { get; set; }

    [CommandOption("--sort <sort>")]
    [DefaultValue(MessageSort.TimeSent)]
    [Description("The sort order to use when searching messages. Allowed values: TimeSent, ProcessingTime, CriticalTime, DeliveryTime")]
    public MessageSort Sort { get; set; }

    [CommandOption("--sort-direction <sort-direction>")]
    [DefaultValue(SortDirection.Desc)]
    [Description("The sort direction to use when searching messages. Allowed values: Asc, Desc")]
    public SortDirection SortDirection { get; set; }

    public override ValidationResult Validate()
    {
        var baseResult = base.Validate();
        if (baseResult.Successful == false) return baseResult;

        if (To is not null && From is null || To is null && From is not null)
        {
            return ValidationResult.Error("Both --from and --to must be provided together.");
        }

        return ValidationResult.Success();
    }
}

public enum MessageSort
{
    [Description("time_sent")]
    TimeSent,
    [Description("processing_time")]
    ProcessingTime,
    [Description("critical_time")]
    CriticalTime,
    [Description("delivery_time")]
    DeliveryTime,
}

public enum SortDirection
{
    [Description("desc")]
    Desc,
    [Description("asc")]
    Asc,
}