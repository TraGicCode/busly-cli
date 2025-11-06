namespace NServiceBusCLI.Config;

public class RabbitmqTransportConfig : ITransportConfig
{
    // TODO: Test TLS Connections to broker using "amqps://your-username:your-password@your.rabbitmq.host:5671/vhost"
    public string AmqpConnectionString { get; set; }

    // TODO: Add Support for TLS Client Certificate Authentication
    // https://github.com/Particular/NServiceBus.RabbitMQ/blob/master/src/NServiceBus.Transport.RabbitMQ/Connection/ConnectionFactory.cs#L69

    public ManagementApi ManagementApi { get; set; }
}

public class ManagementApi
{
    public string Url { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}