namespace BuslyCLI;

public static class Constants
{
    public const string DefaultOriginatingEndpoint = "BuslyCLI";
    public const string DefaultConfigPath = "~/.nservicebus/config.yaml";

    public static class NServiceBus
    {
        public const string RabbitMQTransportName = "rabbitmq";
        public const string AmazonSqsTransportName = "amazonsqs";
        public const string AzureServiceBusTransportName = "azureservicebus";
        public const string LearningTransportName = "learning";
        public const string CommandMessageIntent = "Send";
        public const string EventMessageIntent = "Publish";
    }
}