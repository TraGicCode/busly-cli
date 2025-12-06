namespace BuslyCLI.Config;


public class AmazonsqsTransportConfig : ITransportConfig
{

    //  Local Stack Only
    public string ServiceUrl { get; set; }
    public string RegionName { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }

    public AwsS3BucketSettings S3BucketSettings { get; set; }

}

public class AwsS3BucketSettings
{
    public string BucketName { get; set; }
    public string KeyPrefix { get; set; }
}