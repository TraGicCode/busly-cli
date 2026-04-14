namespace BuslyCLI.Config.Transports;

public class AzureServiceBusTransportConfig : ITransportConfig
{
    public string ConnectionString { get; set; }
}