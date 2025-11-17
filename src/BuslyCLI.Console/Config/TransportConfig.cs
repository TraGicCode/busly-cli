using YamlDotNet.Serialization;

namespace BuslyCLI.Config;

public class TransportConfig
{
    public string Name { get; set; }
    public LearningTransportConfig LearningTransportConfig { get; set; }
    public RabbitmqTransportConfig RabbitmqTransportConfig { get; set; }
    public AmazonsqsTransportConfig AmazonsqsTransportConfig { get; set; }
    public AzureServiceBusTransportConfig AzureServiceBusTransportConfig { get; set; }

    // Helper property to unify config access:
    [YamlIgnore]
    public ITransportConfig Config => (ITransportConfig)LearningTransportConfig
                                      ?? (ITransportConfig)RabbitmqTransportConfig
                                      ?? (ITransportConfig)AmazonsqsTransportConfig
                                      ?? (ITransportConfig)AzureServiceBusTransportConfig;
}