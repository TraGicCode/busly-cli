using NServiceBusCLI.Config;

namespace NServiceBusCLI.Factories;

public interface IRawEndpointFactory
{
    Task<RawEndpoint> CreateRawEndpoint(string endpointName, TransportConfig transportConfig);

    Task<RawSendOnlyEndpoint> CreateRawSendOnlyEndpoint(string endpointName, TransportConfig transportConfig);
}