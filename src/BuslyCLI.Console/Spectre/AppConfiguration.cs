using BuslyCLI.Commands.Command;
using BuslyCLI.Commands.Demo;
using BuslyCLI.Commands.Event;
using BuslyCLI.Commands.Transport;
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
            config.SetApplicationVersion("1.0.0");
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
            config.AddBranch("demo", demo =>
            {
                demo.SetDescription("Demo mode for the busly quick start guide.");
                demo.AddCommand<StartDemoCommand>("start")
                    .WithDescription("Start a demo endpoint that can receive any command and a single 'Messages.Events.OrderPlaced' event.");
            });
            config.PropagateExceptions();
        };
    }
}