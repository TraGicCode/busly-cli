namespace BuslyCLI.Config;

public class AzureStorageQueuesTransportConfig : ITransportConfig
{
    public string ConnectionString { get; set; }
}