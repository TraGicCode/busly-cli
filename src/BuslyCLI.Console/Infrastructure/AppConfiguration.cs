using System.Reflection;
using BuslyCLI.Commands.Demo;
using BuslyCLI.Commands.NsbCommand;
using BuslyCLI.Commands.NsbEvent;
using BuslyCLI.Commands.NsbTimeout;
using BuslyCLI.Commands.ServiceControl.CustomCheck;
using BuslyCLI.Commands.ServiceControl.Endpoint;
using BuslyCLI.Commands.ServiceControl.Event;
using BuslyCLI.Commands.ServiceControl.Instance;
using BuslyCLI.Commands.ServiceControl.Message;
using BuslyCLI.Commands.Transport;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Infrastructure;

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
            config.AddBranch("servicecontrol", servicecontrol =>
            {
                servicecontrol.SetDescription("Interact with Service Control Instances.");
                servicecontrol.AddBranch("instance", instance =>
                {
                    instance.SetDescription("Manage ServiceControl instance configurations.");
                    instance.AddCommand<ListServiceControlInstancesCommand>("list")
                        .WithAlias("ls")
                        .WithDescription("List all configured ServiceControl instances.");
                    instance.AddCommand<CurrentServiceControlInstanceCommand>("current")
                        .WithAlias("c")
                        .WithDescription("Display the current ServiceControl instance.");
                    instance.AddCommand<SetServiceControlInstanceCommand>("set")
                        .WithAlias("s")
                        .WithDescription("Set the current ServiceControl instance.");
                    instance.AddCommand<DeleteServiceControlInstanceCommand>("delete")
                        .WithAlias("d")
                        .WithDescription("Delete a configured ServiceControl instance.");
                });
                servicecontrol.AddBranch("endpoint", endpoints =>
                {
                    endpoints.SetDescription("Manage ServiceControl endpoints.");
                    endpoints.AddCommand<ListEndpointsCommand>("list")
                        .WithAlias("ls")
                        .WithDescription("List all NServiceBus endpoints ServiceControl knows about.");
                    endpoints.AddCommand<DeleteEndpointCommand>("delete")
                        .WithAlias("d")
                        .WithDescription("Delete/Decommission an endpoint from the current ServiceControl instance.");
                });
                servicecontrol.AddBranch("message", message =>
                {
                    message.SetDescription("Search for messages sent by endpoints.");
                    message.AddCommand<SearchMessagesCommand>("search")
                        .WithAlias("s")
                        .WithDescription("Search for successful messages.");
                });
                servicecontrol.AddBranch("license", message =>
                {
                    message.SetDescription("Show license information for the current ServiceControl instance.");
                    message.AddCommand<ShowLicenseCommand>("show")
                        .WithAlias("s")
                        .WithDescription("Show license information for the current ServiceControl instance.");
                });
                servicecontrol.AddBranch("event", @event =>
                {
                    @event.SetDescription("Show events for the current ServiceControl instance.");
                    @event.AddCommand<ListEventsCommand>("list")
                        .WithAlias("ls")
                        .WithDescription("Show events for the current ServiceControl instance.");
                });
                servicecontrol.AddBranch("custom-check", customChecks =>
                {
                    customChecks.SetDescription("Manage custom checks for the current ServiceControl instance.");
                    customChecks.AddCommand<ListCustomChecksCommand>("list")
                        .WithAlias("ls")
                        .WithDescription("Show custom-checks for the current ServiceControl instance.");
                });
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