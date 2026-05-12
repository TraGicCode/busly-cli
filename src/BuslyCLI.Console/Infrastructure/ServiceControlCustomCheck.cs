namespace BuslyCLI.Infrastructure.ServiceControl;

public class ServiceControlCustomCheck
{
    public string Id { get; set; }
    public string CustomCheckId { get; set; }
    public string Category { get; set; }
    public string Status { get; set; }
    public DateTimeOffset ReportedAt { get; set; }
    public string FailureReason { get; set; }
    public ServiceControlCustomCheckEndpoint OriginatingEndpoint { get; set; }
}

public class ServiceControlCustomCheckEndpoint
{
    public string Name { get; set; }
    public Guid HostId { get; set; }
    public string Host { get; set; }
}