using System.Text;
using BuslyCLI.Config;
using BuslyCLI.Infrastructure.Factories;
using NServiceBus.Routing;
using NServiceBus.Transport;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.NsbCommand;

public class SendCommand(IRawEndpointFactory rawEndpointFactory, INServiceBusConfiguration nServiceBusConfiguration) : AsyncCommand<SendCommandSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, SendCommandSettings settings, CancellationToken cancellationToken)
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
            Encoding.ASCII.GetBytes(settings.MessageBody.Value)
        );

        var transportOperation = new TransportOperation(
            message,
            new UnicastAddressTag(settings.DestinationEndpoint)
        );

        await rawEndpoint.Dispatch(
            new TransportOperations(transportOperation),
            new TransportTransaction()
        );

        await rawEndpoint.ShutDownAndCleanUp();

        return 0;
    }
}