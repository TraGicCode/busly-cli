using System.Text;
using BuslyCLI.Commands.Transport;
using BuslyCLI.Config;
using BuslyCLI.Infrastructure.Factories;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.Demo;

public class StartDemoCommand(IAnsiConsole console, IRawEndpointFactory rawEndpointFactory, INServiceBusConfiguration nServiceBusConfiguration)
    : AsyncCommand<CurrentTransportSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CurrentTransportSettings settings, CancellationToken cancellationToken)
    {
        console.WriteLine($"Starting demo endpoint named {Constants.DemoDefaultOriginatingEndpoint} for quick start guide...");
        var config = await nServiceBusConfiguration.GetValidatedConfigurationAsync(settings.Config.Path);
        var rawEndpoint = await rawEndpointFactory.CreateRawEndpoint(Constants.DemoDefaultOriginatingEndpoint, config.CurrentTransportConfig);

        await rawEndpoint.StartEndpoint();
        console.WriteLine($"{Constants.DemoDefaultOriginatingEndpoint} Endpoint Started.");
        await rawEndpoint.Subscribe("Messages.Events.OrderPlaced", cancellationToken);

        do
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            var receivedMessage = rawEndpoint.TryReceiveMessage();
            if (receivedMessage == null) continue;
            console.WriteLine("===============================================================");
            console.WriteLine($"{(receivedMessage.Headers["NServiceBus.MessageIntent"] == "Send" ? "Command" : "Event")} Received");
            console.WriteLine("MessageId: " + receivedMessage.Headers["NServiceBus.MessageId"]);
            console.WriteLine("MessageType: " + receivedMessage.Headers["NServiceBus.EnclosedMessageTypes"]);
            console.WriteLine("Body: " + Encoding.UTF8.GetString(receivedMessage.Body.Span));
            console.WriteLine("===============================================================");
        } while (true);
        // ReSharper disable once FunctionNeverReturns
    }
}