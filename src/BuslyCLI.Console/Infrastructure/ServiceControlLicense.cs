namespace BuslyCLI.Infrastructure.ServiceControl;

public class ServiceControlLicense
{
    public bool TrialLicense { get; set; }
    public string Edition { get; set; }
    public string RegisteredTo { get; set; }
    public string UpgradeProtectionExpiration { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public string Status { get; set; }
    public string LicenseType { get; set; }
    public string InstanceName { get; set; }
    public string LicenseStatus { get; set; }
    public string LicenseExtensionUrl { get; set; }
}