namespace BuslyCLI.Infrastructure.ServiceControl;

public class ServiceControlMessage
{
    public string Id { get; set; }
    public string MessageId { get; set; }
    public string MessageType { get; set; }
    public ServiceControlMessageEndpoint SendingEndpoint { get; set; }
    public ServiceControlMessageEndpoint ReceivingEndpoint { get; set; }
    public DateTimeOffset? TimeSent { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public string CriticalTime { get; set; }
    public string DeliveryTime { get; set; }
    public string ProcessingTime { get; set; }
    public bool IsSystemMessage { get; set; }
    public string ConversationId { get; set; }
    public string Status { get; set; }
    public string MessageIntent { get; set; }
    public string BodyUrl { get; set; }
    public int BodySize { get; set; }
    public string InstanceId { get; set; }
    public IReadOnlyList<ServiceControlMessageHeader> Headers { get; set; } = [];
}

public class ServiceControlMessageEndpoint
{
    public string Name { get; set; }
    public string Host { get; set; }
    public string HostId { get; set; }
}

public class ServiceControlMessageHeader
{
    public string Key { get; set; }
    public string Value { get; set; }
}