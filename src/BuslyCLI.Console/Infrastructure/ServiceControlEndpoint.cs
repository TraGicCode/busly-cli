namespace BuslyCLI.Infrastructure.ServiceControl;

public class ServiceControlEndpoint
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string HostDisplayName { get; set; }
    public bool Monitored { get; set; }
    public bool MonitorHeartbeat { get; set; }
    public HeartbeatInformation HeartbeatInformation { get; set; }
    public bool IsSendingHeartbeats { get; set; }
}

public class HeartbeatInformation
{
    public DateTimeOffset LastReportAt { get; set; }
    public string ReportedStatus { get; set; }
}