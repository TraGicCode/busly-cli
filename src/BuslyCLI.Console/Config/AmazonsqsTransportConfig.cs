namespace BuslyCLI.Config;


public class AmazonsqsTransportConfig : ITransportConfig
{

    //  Local Stack Only
    public string ServiceUrl { get; set; }
    public string RegionName { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }

}