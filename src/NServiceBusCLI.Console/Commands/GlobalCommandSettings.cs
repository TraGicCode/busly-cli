using System.ComponentModel;
using NServiceBusCLI.TypeConverters;
using Spectre.Console.Cli;

namespace NServiceBusCLI.Commands;

public class GlobalCommandSettings : CommandSettings
{
    // [CommandOption("--verbose")]
    // public bool Verbose { get; set; }

    [CommandOption("--config <FILE>")]
    [Description("Path to the config.yaml file to use for the CLI.")]
    [DefaultValue(Constants.DefaultConfigPath)]
    public ExpandedPath Config { get; set; }
}