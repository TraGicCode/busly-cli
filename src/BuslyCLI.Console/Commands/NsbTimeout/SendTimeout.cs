using System.Text;
using BuslyCLI.Config;
using BuslyCLI.Config.Transports;
using BuslyCLI.Infrastructure.Factories;
using NServiceBus.DelayedDelivery;
using NServiceBus.Routing;
using NServiceBus.Transport;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.NsbTimeout;

public class SendTimeout(IAnsiConsole console, IRawEndpointFactory rawEndpointFactory, INServiceBusConfiguration nServiceBusConfiguration) : AsyncCommand<SendTimeoutCommandSettings>
{
    private static readonly HashSet<Type> UnsupportedTransportTypes =
    [
        typeof(SqlServerTransportConfig),
        typeof(PostgreSqlTransportConfig),
        typeof(AzureStorageQueuesTransportConfig)
    ];

    protected override async Task<int> ExecuteAsync(CommandContext context, SendTimeoutCommandSettings settings, CancellationToken cancellationToken)
    {
        var config = await nServiceBusConfiguration.GetValidatedConfigurationAsync(settings.Config.Path);

        if (UnsupportedTransportTypes.Contains(config.CurrentTransportConfig.Config.GetType()))
        {
            console.MarkupLine($"[red]Error:[/] The [bold]{config.CurrentTransportConfig.Config.GetType().Name.Replace("Config", "")}[/] transport does not support sending timeouts.");
            console.MarkupLine("This transport relies on an in-process poller to forward deferred messages, which is incompatible with the CLI's fire-and-forget execution model.");
            console.MarkupLine("For details see: [link]https://tragiccode.com/busly-cli/docs/cli-reference/timeout/send[/]");
            return 1;
        }

        var rawEndpoint = await rawEndpointFactory.CreateRawSendOnlyEndpoint(Constants.DefaultOriginatingEndpoint, config.CurrentTransportConfig);
        // TODO: Validate body is valid json/xml
        var headers = new Dictionary<string, string>
        {
            ["NServiceBus.OriginatingEndpoint"] = Constants.DefaultOriginatingEndpoint,
            ["NServiceBus.OriginatingMachine"] = Environment.MachineName,
            ["NServiceBus.ConversationId"] = Guid.NewGuid().ToString(),
            ["NServiceBus.CorrelationId"] = Guid.NewGuid().ToString(),
            ["NServiceBus.MessageIntent"] = Constants.NServiceBus.CommandMessageIntent,
            ["NServiceBus.ContentType"] = settings.ContentType,
            ["NServiceBus.EnclosedMessageTypes"] = settings.EnclosedMessageType
        };
        var message = new OutgoingMessage(
            Guid.NewGuid().ToString(),
            headers,
            Encoding.ASCII.GetBytes(settings.MessageBody.Value)
        );

        var dispatchProperties = new DispatchProperties();

        if (settings.DoNotDeliverBefore is not null)
        {
            dispatchProperties.DoNotDeliverBefore = new DoNotDeliverBefore(settings.DoNotDeliverBefore.Value);
        }
        else if (settings.DelayDeliveryWith is not null)
        {
            dispatchProperties.DelayDeliveryWith = new DelayDeliveryWith(settings.DelayDeliveryWith.Value);
        }

        var transportOperation = new TransportOperation(
            message,
            new UnicastAddressTag(settings.DestinationEndpoint),
            dispatchProperties
        );

        await rawEndpoint.Dispatch(
            new TransportOperations(transportOperation),
            new TransportTransaction(),
            cancellationToken);

        await rawEndpoint.ShutDownAndCleanUp();

        return 0;
    }
}