using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using BuslyCLI.Config;
using BuslyCLI.Infrastructure.Factories;
using NServiceBus.Routing;
using NServiceBus.Transport;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.NsbEvent;

public class PublishCommand(IRawEndpointFactory rawEndpointFactory, INServiceBusConfiguration nServiceBusConfiguration) : AsyncCommand<PublishCommandSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, PublishCommandSettings settings, CancellationToken cancellationToken)
    {
        var config = await nServiceBusConfiguration.GetValidatedConfigurationAsync(settings.Config.Path);
        var rawEndpoint = await rawEndpointFactory.CreateRawSendOnlyEndpoint(Constants.DefaultOriginatingEndpoint, config.CurrentTransportConfig);
        // TODO: Validate body is valid json/xml
        var headers = new Dictionary<string, string>
        {
            // TODO: How do i get the name from the SendOnly Endpoint?
            ["NServiceBus.OriginatingEndpoint"] = Constants.DefaultOriginatingEndpoint,
            ["NServiceBus.OriginatingMachine"] = Environment.MachineName,
            ["NServiceBus.ConversationId"] = Guid.NewGuid().ToString(),
            ["NServiceBus.CorrelationId"] = Guid.NewGuid().ToString(),
            ["NServiceBus.MessageIntent"] = Constants.NServiceBus.EventMessageIntent,
            ["NServiceBus.ContentType"] = settings.ContentType,
            ["NServiceBus.EnclosedMessageTypes"] = settings.EnclosedMessageType
        };
        var message = new OutgoingMessage(
            Guid.NewGuid().ToString(),
            headers,
            Encoding.ASCII.GetBytes(settings.MessageBody.Value)
        );

        var type = CreateTypeFromString(settings.EnclosedMessageType);

        var transportOperation = new TransportOperation(
            message,
            new MulticastAddressTag(type)
        );

        await rawEndpoint.Dispatch(
            new TransportOperations(transportOperation),
            new TransportTransaction()
        );

        await rawEndpoint.ShutDownAndCleanUp();

        return 0;
    }

    private static Type CreateTypeFromString(string typeAsString)
    {
        var typeSignature = typeAsString;
        var assemblyBuilder =
            AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

        var type = moduleBuilder.DefineType(typeSignature,
            TypeAttributes.Public |
            TypeAttributes.Class |
            TypeAttributes.AutoClass |
            TypeAttributes.AnsiClass |
            TypeAttributes.BeforeFieldInit |
            TypeAttributes.AutoLayout,
            null).GetTypeInfo().AsType();
        return type;
    }
}