

using System.ComponentModel;
using BuslyCLI.Commands;
using Spectre.Console.Cli;

public class ListCustomChecksSettings : GlobalCommandSettings
{
    [CommandOption("--page-size <page-size>")]
    [DefaultValue(50)]
    [Description("The page size to use when searching events")]
    public int PageSize { get; set; }

    [CommandOption("--page-number <page-number>")]
    [DefaultValue(1)]
    [Description("The page number to use when searching events")]
    public int Page { get; set; }
}