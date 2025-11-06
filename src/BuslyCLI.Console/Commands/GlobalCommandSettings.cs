using System.ComponentModel;
using BuslyCLI.TypeConverters;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands;

public class GlobalCommandSettings : CommandSettings
{
    // [CommandOption("--verbose")]
    // public bool Verbose { get; set; }

    [CommandOption("--config <FILE>")]
    [Description("Path to the config.yaml file to use for the CLI.")]
    [DefaultValue(Constants.DefaultConfigPath)]
    public ExpandedPath Config { get; set; }
}