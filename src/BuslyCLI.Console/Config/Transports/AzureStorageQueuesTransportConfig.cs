namespace BuslyCLI.Config.Transports;

public class AzureStorageQueuesTransportConfig : ITransportConfig
{
    public string ConnectionString { get; set; }
}