using NServiceBus.Transport;

namespace BuslyCLI.Infrastructure.Endpoints;

public class RawSendOnlyEndpoint
{
    protected readonly TransportInfrastructure _infrastructure;
    public string EndpointName { get; }


    public RawSendOnlyEndpoint(TransportInfrastructure infrastructure, string endpointName)
    {
        _infrastructure = infrastructure;
        EndpointName = endpointName;
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