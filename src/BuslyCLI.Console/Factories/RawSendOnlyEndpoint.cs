using NServiceBus.Transport;

namespace BuslyCLI.Factories;

public class RawSendOnlyEndpoint
{
    protected readonly TransportInfrastructure _infrastructure;


    public RawSendOnlyEndpoint(TransportInfrastructure infrastructure)
    {
        _infrastructure = infrastructure;
    }

    public async Task Dispatch(TransportOperations outgoingMessages, TransportTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        await _infrastructure.Dispatcher.Dispatch(outgoingMessages, transaction, cancellationToken);
    }

    public virtual async Task ShutDownAndCleanUp()
    {
        await _infrastructure.Shutdown();
    }
}