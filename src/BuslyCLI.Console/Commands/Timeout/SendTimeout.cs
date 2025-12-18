using System.Text;
using BuslyCLI.Config;
using BuslyCLI.Factories;
using NServiceBus.DelayedDelivery;
using NServiceBus.Routing;
using NServiceBus.Transport;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.Timeout;

public class SendTimeout(IRawEndpointFactory rawEndpointFactory, INServiceBusConfiguration nServiceBusConfiguration) : AsyncCommand<SendTimeoutCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, SendTimeoutCommandSettings settings, CancellationToken cancellationToken)
    {
        var config = await nServiceBusConfiguration.GetValidatedConfigurationAsync(settings.Config.Path);
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
            Encoding.ASCII.GetBytes(settings.MessageBody)
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