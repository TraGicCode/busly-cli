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

    public Task Dispatch(TransportOperations outgoingMessages, TransportTransaction transaction,
        CancellationToken cancellationToken = default)
        => _infrastructure.Dispatcher.Dispatch(outgoingMessages, transaction, cancellationToken);

    public virtual Task ShutDownAndCleanUp()
        => _infrastructure.Shutdown();
}