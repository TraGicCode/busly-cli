namespace NServiceBusCLI.Config;

public class AzureServiceBusTransportConfig : ITransportConfig
{
    public string ConnectionString { get; set; }
}