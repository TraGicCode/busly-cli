using System.Reflection;
using BuslyCLI.Commands.Command;
using BuslyCLI.Commands.Demo;
using BuslyCLI.Commands.Event;
using BuslyCLI.Commands.Timeout;
using BuslyCLI.Commands.Transport;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Spectre;

public static class AppConfiguration
{
    public static Action<IConfigurator> GetSpectreCommandConfiguration()
    {
        return config =>
        {
            config.SetApplicationName("busly");
            // TODO: Allow this to get set via the CLI version
            var assembly = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            config.SetApplicationVersion(assembly.InformationalVersion);
            config.AddBranch("transport", transport =>
            {
                transport.SetDescription("Manage transport configurations.");
                transport.AddCommand<ListTransportsCommand>("list")
                    .WithAlias("ls")
                    .WithDescription("List all configured transports.");
                transport.AddCommand<CurrentTransportCommand>("current")
                    .WithAlias("c")
                    .WithDescription("Display current transport.");
                transport.AddCommand<DeleteTransportCommand>("delete")
                    .WithAlias("d")
                    .WithDescription("Delete a configured transport.");
                transport.AddCommand<SetTransportCommand>("set")
                    .WithAlias("s")
                    .WithDescription("Set the current transport.");
            });
            config.AddBranch("command", command =>
            {
                command.SetDescription("Operations related to NServiceBus commands.");
                command.AddCommand<SendCommand>("send")
                    .WithAlias("s")
                    .WithDescription("Send a one-way command to an endpoint.");
            });
            config.AddBranch("event", @event =>
            {
                @event.SetDescription("Operations related to NServiceBus events.");
                @event.AddCommand<PublishCommand>("publish")
                    .WithAlias("p")
                    .WithDescription("Publish an event to subscribing endpoints.");
            });
            config.AddBranch("timeout", timeout =>
            {
                timeout.SetDescription("Operations related to NServiceBus timeouts.");
                timeout.AddCommand<SendTimeout>("send")
                    .WithAlias("s")
                    .WithDescription("Send a timeout message to an endpoint.");
            });
            config.AddBranch("demo", demo =>
            {
                demo.SetDescription("Demo mode for the busly quick start guide.");
                demo.AddCommand<StartDemoCommand>("start")
                    .WithDescription("Start a demo endpoint that can receive any command and a single 'Messages.Events.OrderPlaced' event.");
            });

            config.SetExceptionHandler((ex, _) =>
            {
                // if (ex.InnerException is OptionsValidationException)
                // {
                //     AnsiConsole.Write(new Markup($"{ConsoleExtensions.ErrorMarkup}{ex.InnerException.Message}"));
                //     return;
                // }
                // if (ex is CommandAppException)
                // {
                //     AnsiConsole.Write(new Markup($"{ConsoleExtensions.ErrorMarkup}{ex.Message}"));
                //     return;
                // }
                // AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths);
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            });


        };
    }
}