using BuslyCLI.Infrastructure.Endpoints;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Infrastructure.Factories;

public interface IRawEndpointFactory
{
    Task<RawEndpoint> CreateRawEndpoint(string endpointName, TransportConfig transportConfig, bool setupInfrastructure = true);

    Task<RawSendOnlyEndpoint> CreateRawSendOnlyEndpoint(string endpointName, TransportConfig transportConfig);
}