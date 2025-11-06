namespace NServiceBusCLI.Config;


public class AmazonsqsTransportConfig : ITransportConfig
{
    public string ServiceUrl { get; set; }
    public string RegionName { get; set; }
}