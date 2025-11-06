using NServiceBusCLI.Commands.Command;
using NServiceBusCLI.Commands.Event;
using NServiceBusCLI.Commands.Transport;
using Spectre.Console.Cli;

namespace NServiceBusCLI.Spectre;

public static class AppConfiguration
{
    public static Action<IConfigurator> GetSpectreCommandConfiguration()
    {
        return config =>
        {
            config.SetApplicationName("nservicebus");
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
            config.PropagateExceptions();
        };
    }
}