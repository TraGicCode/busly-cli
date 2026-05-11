using System.ComponentModel;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.ServiceControl.Event;

public class ListEventsSettings : GlobalCommandSettings
{
    [CommandOption("--page-size <page-size>")]
    [DefaultValue(50)]
    [Description("The page size to use when searching events")]
    public int PageSize { get; set; }

    [CommandOption("--page-number <page-number>")]
    [DefaultValue(1)]
    [Description("The page number to use when searching events")]
    public int PageNumber { get; set; }
}