using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Factories;

public interface IRawEndpointFactory
{
    Task<RawEndpoint> CreateRawEndpoint(string endpointName, TransportConfig transportConfig, bool setupInfrastructure = true);

    Task<RawSendOnlyEndpoint> CreateRawSendOnlyEndpoint(string endpointName, TransportConfig transportConfig);
}