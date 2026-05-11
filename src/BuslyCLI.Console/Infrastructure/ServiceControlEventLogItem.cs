namespace BuslyCLI.Infrastructure.ServiceControl;

public class ServiceControlEventLogItem
{
    public string Id { get; set; }
    public string Description { get; set; }
    public string Severity { get; set; }
    public DateTimeOffset RaisedAt { get; set; }
    public List<string> RelatedTo { get; set; }
    public string Category { get; set; }
    public string EventType { get; set; }
}