namespace BuslyCLI.Config;

public class AzureServiceBusTransportConfig : ITransportConfig
{
    public string ConnectionString { get; set; }
}