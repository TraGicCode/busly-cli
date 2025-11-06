# CLI Reference

This document contains information about all commands and settings available in the Busly CLI, including examples and flags. To get started with the Busly CLI, see Get Started.

## Core Commands

-   `busly transport current`
-   `busly transport set`
-   `busly command send`
-   `busly event publish`

## Global Options

The following global flags can be used with any command:

-   `-h`, `--help`: Prints help information.
-   `--config`: Path to the `config.yaml` file to use for the CLI. (Default: `~/.busly-cli/config.yaml` (Windows and MacOS) and `~/home/.busly-cli/config.yaml` (Linux))

## Top Level Options

The following top level flags can be used at the top level ( when no command is passed ):

-   `-v`, `--version`: Prints version information.
