using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using NServiceBus.Extensibility;
using NServiceBus.Transport;
using NServiceBus.Unicast.Messages;

namespace NServiceBusCLI.Console.Tests.EndToEnd.Infrastructure;

public class TestEndpoint
{
    private static readonly TimeSpan IncomingMessageTimeout = TimeSpan.FromSeconds(5);
    private readonly TransportInfrastructure _infrastructure;
    private readonly BlockingCollection<IncomingMessage> _receivedMessages = new();
    private IMessageReceiver _messageReceiver;
    private ISubscriptionManager _subscriptionManager;

    public TestEndpoint(TransportInfrastructure infrastructure)
    {
        _infrastructure = infrastructure;
    }

    /// <summary>
    ///     Gets the name of this test endpoint.
    /// </summary>
    public string EndpointName => _messageReceiver.ReceiveAddress;

    public async Task StartEndpoint()
    {
        _messageReceiver = _infrastructure.Receivers["Primary"];
        _subscriptionManager = _messageReceiver.Subscriptions;
        await _messageReceiver.Initialize(new PushRuntimeSettings(1),
            OnMessage,
            OnError);

        await _messageReceiver.StartReceive();
    }

    public async Task StopReceive()
    {
        await _messageReceiver.StopReceive();
    }

    public async Task ShutDownAndCleanUp()
    {
        await _messageReceiver.StopReceive();
        await _infrastructure.Shutdown();
    }

    private static Type CreateTypeFromString(string typeAsString)
    {
        var typeSignature = typeAsString;
        var an = new AssemblyName(typeSignature);
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

    public async Task Subscribe(string eventType, CancellationToken cancellationToken = default)
    {
        await _subscriptionManager.SubscribeAll([new MessageMetadata(CreateTypeFromString(eventType))],
            new ContextBag(), cancellationToken);
    }

    public Task OnMessage(MessageContext messageContext, CancellationToken cancellationToken)
    {
        _receivedMessages.Add(
            new IncomingMessage(messageContext.NativeMessageId, messageContext.Headers, messageContext.Body),
            cancellationToken);
        return Task.CompletedTask;
    }

    public Task<ErrorHandleResult> OnError(ErrorContext errorContext, CancellationToken cancellationToken)
    {
        return Task.FromResult(ErrorHandleResult.RetryRequired);
    }

    public IncomingMessage TryReceiveMessage()
    {
        if (_receivedMessages.TryTake(out var incomingMessage, IncomingMessageTimeout)) return incomingMessage;
        throw new TimeoutException($"The message did not arrive within {IncomingMessageTimeout.TotalSeconds} seconds.");
    }
}