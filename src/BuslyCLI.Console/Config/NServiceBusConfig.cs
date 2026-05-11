using YamlDotNet.Serialization;

namespace BuslyCLI.Config;

public class NServiceBusConfig
{
    public string CurrentTransport { get; set; }
    public ICollection<TransportConfig> Transports { get; set; }

    [YamlIgnore]
    public TransportConfig CurrentTransportConfig => Transports.FirstOrDefault(x => x.Name == CurrentTransport);

    public string CurrentServiceControlInstance { get; set; }
    public ICollection<ServiceControlInstanceConfig> ServiceControlInstances { get; set; }

    [YamlIgnore]
    public ServiceControlInstanceConfig CurrentServiceControlInstanceConfig =>
        ServiceControlInstances?.FirstOrDefault(x => x.Name == CurrentServiceControlInstance);
}