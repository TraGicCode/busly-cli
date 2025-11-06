using BuslyCLI.Config;

namespace BuslyCLI.Factories;

public interface IRawEndpointFactory
{
    Task<RawEndpoint> CreateRawEndpoint(string endpointName, TransportConfig transportConfig);

    Task<RawSendOnlyEndpoint> CreateRawSendOnlyEndpoint(string endpointName, TransportConfig transportConfig);
}