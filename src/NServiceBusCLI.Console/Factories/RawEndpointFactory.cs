using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using NServiceBus.Transport;
using NServiceBusCLI.Config;

namespace NServiceBusCLI.Factories;

public class RawEndpointFactory : IRawEndpointFactory
{
    public async Task<RawEndpoint> CreateRawEndpoint(string endpointName, TransportConfig transportConfig)
    {
        var transport = CreateTransport(transportConfig);
        return await InternalCreateEndpoint(endpointName, transport);
    }

    public async Task<RawSendOnlyEndpoint> CreateRawSendOnlyEndpoint(string endpointName, TransportConfig transportConfig)
    {
        var transport = CreateTransport(transportConfig);
        return await InternalCreateSendOnlyEndpoint(endpointName, transport);
    }

    private TransportDefinition CreateTransport(TransportConfig transportConfig)
    {
        switch (transportConfig.Config)
        {
            case RabbitmqTransportConfig rabbitmqTransportConfig:
                return CreateRabbitMQTransport(rabbitmqTransportConfig);
            case AzureServiceBusTransportConfig azureServiceBusTransportConfig:
                return CreateAzureServiceBusTransport(azureServiceBusTransportConfig.ConnectionString);
            case AmazonsqsTransportConfig amazonSqsTransportConfig:
                return CreateAmazonSQSTransport(amazonSqsTransportConfig);
            case LearningTransportConfig learningTransportConfig:
                return new LearningTransport
                {
                    StorageDirectory = learningTransportConfig.StorageDirectory,
                    RestrictPayloadSize = learningTransportConfig.RestrictPayloadSize
                };
            default:
                throw new ApplicationException("Unknown transport type: " + transportConfig.Config.GetType().Name);
        }
    }

    private RabbitMQTransport CreateRabbitMQTransport(RabbitmqTransportConfig rabbitmqTransportConfig)
    {
        var t = new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum),
            rabbitmqTransportConfig.AmqpConnectionString)
        {
            ManagementApiConfiguration = CreateManagementApiConfig(rabbitmqTransportConfig.ManagementApi)
        };
        return t;
    }

    private static ManagementApiConfiguration CreateManagementApiConfig(ManagementApi rabbitmqManagementApi)
    {
        var hasUrl = !string.IsNullOrWhiteSpace(rabbitmqManagementApi.Url);
        var hasCreds = !string.IsNullOrWhiteSpace(rabbitmqManagementApi.UserName) && !string.IsNullOrWhiteSpace(rabbitmqManagementApi.Password);

        if (hasUrl && hasCreds)
            return new ManagementApiConfiguration(rabbitmqManagementApi.Url, rabbitmqManagementApi.UserName, rabbitmqManagementApi.Password);

        if (hasUrl)
            return new ManagementApiConfiguration(rabbitmqManagementApi.Url);

        if (hasCreds)
            return new ManagementApiConfiguration(rabbitmqManagementApi.UserName, rabbitmqManagementApi.Password);

        return null;
    }


    private TransportDefinition CreateAmazonSQSTransport(AmazonsqsTransportConfig amazonsqsTransportConfig)
    {
        var credentials = new BasicAWSCredentials("test", "test");

        var sqsClient = new AmazonSQSClient(credentials, new AmazonSQSConfig
        {
            ServiceURL = amazonsqsTransportConfig.ServiceUrl,
            AuthenticationRegion = amazonsqsTransportConfig.RegionName,
        });

        var snsClient = new AmazonSimpleNotificationServiceClient(credentials, new AmazonSimpleNotificationServiceConfig
        {
            ServiceURL = amazonsqsTransportConfig.ServiceUrl,
            AuthenticationRegion = amazonsqsTransportConfig.RegionName,
        });

        return new SqsTransport(sqsClient, snsClient);
    }

    private AzureServiceBusTransport CreateAzureServiceBusTransport(string transportConnectionString)
    {
        return new AzureServiceBusTransport(transportConnectionString, TopicTopology.Default);
    }

    private static async Task<RawEndpoint> InternalCreateEndpoint(
        string endpointName,
        TransportDefinition transport)
    {
        var infrastructure = await InternalCreateInfrastructure(
            endpointName,
            transport,
            isReceiveEnabled: true);

        return new RawEndpoint(infrastructure);
    }

    private static async Task<RawSendOnlyEndpoint> InternalCreateSendOnlyEndpoint(
        string endpointName,
        TransportDefinition transport)
    {
        var infrastructure = await InternalCreateInfrastructure(
            endpointName,
            transport,
            isReceiveEnabled: false);

        return new RawSendOnlyEndpoint(infrastructure);
    }

    private static async Task<TransportInfrastructure> InternalCreateInfrastructure(
        string endpointName,
        TransportDefinition transport,
        bool isReceiveEnabled)
    {
        var hostSettings = new HostSettings(
            endpointName,
            endpointName,
            new StartupDiagnosticEntries(),
            (message, exception, token) =>
            {
                // Console.WriteLine("Critical error: " + exception);
            },
            isReceiveEnabled);


        var infrastructure = await transport.Initialize(hostSettings, new[]
        {
            new ReceiveSettings(
                "Primary",
                new QueueAddress(endpointName),
                isReceiveEnabled,
                false,
                "error")
        }, new string[0]);


        return infrastructure;
    }
}