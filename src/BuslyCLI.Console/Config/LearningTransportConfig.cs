namespace BuslyCLI.Config;

public class LearningTransportConfig : ITransportConfig
{
    public string StorageDirectory { get; set; }
    public bool RestrictPayloadSize { get; set; } = true;
}