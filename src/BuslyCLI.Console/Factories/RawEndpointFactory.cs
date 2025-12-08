using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using BuslyCLI.Config;
using NServiceBus.Transport;

namespace BuslyCLI.Factories;

public class RawEndpointFactory : IRawEndpointFactory
{
    public async Task<RawEndpoint> CreateRawEndpoint(string endpointName, TransportConfig transportConfig, bool setupInfrastructure = true)
    {
        var transport = CreateTransport(transportConfig);
        return await InternalCreateEndpoint(endpointName, transport, setupInfrastructure);
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
            case AzureStorageQueuesTransportConfig azureStorageQueuesTransportConfig:
                return CreateAzureStorageQueuesTransport(azureStorageQueuesTransportConfig.ConnectionString);
            case AmazonsqsTransportConfig amazonSqsTransportConfig:
                return CreateAmazonSQSTransport(amazonSqsTransportConfig);
            case SqlServerTransportConfig sqlServerTransportConfig:
                return CreateSqlServerTransport(sqlServerTransportConfig);
            case PostgreSqlTransportConfig postgreSqlTransportConfig:
                return CreatePostgreSqlTransport(postgreSqlTransportConfig);
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

    private TransportDefinition CreatePostgreSqlTransport(PostgreSqlTransportConfig postgreSqlTransportConfig)
    {
        var transport = new PostgreSqlTransport(postgreSqlTransportConfig.ConnectionString);
        return transport;
    }

    private TransportDefinition CreateAzureStorageQueuesTransport(string connectionString)
    {
        var transport = new AzureStorageQueueTransport(connectionString, useNativeDelayedDeliveries: false);
        transport.MessageWrapperSerializationDefinition = new SystemJsonSerializer();
        return transport;
    }

    private TransportDefinition CreateSqlServerTransport(SqlServerTransportConfig sqlServerTransportConfig)
    {
        return new SqlServerTransport(sqlServerTransportConfig.ConnectionString);
    }

    private RabbitMQTransport CreateRabbitMQTransport(RabbitmqTransportConfig rabbitmqTransportConfig)
    {
        var routingTopology = rabbitmqTransportConfig.RoutingTopology switch
        {
            RabbitmqRoutingTopology.Conventional => RoutingTopology.Conventional(rabbitmqTransportConfig.QueueType == RabbitmqQueueType.Quorum ? QueueType.Quorum : QueueType.Classic),
            RabbitmqRoutingTopology.Direct => RoutingTopology.Direct(rabbitmqTransportConfig.QueueType == RabbitmqQueueType.Quorum ? QueueType.Quorum : QueueType.Classic),
            _ => throw new Exception("Unknown RabbitMQ routing topology: " + rabbitmqTransportConfig.RoutingTopology)
        };
        var t = new RabbitMQTransport(routingTopology, rabbitmqTransportConfig.AmqpConnectionString);

        if (rabbitmqTransportConfig.ManagementApi != null)
        {
            t.ManagementApiConfiguration = CreateManagementApiConfig(rabbitmqTransportConfig.ManagementApi);
        }
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
        var credentials = new BasicAWSCredentials(amazonsqsTransportConfig.AccessKey, amazonsqsTransportConfig.SecretKey);
        var amazonSqsConfig = new AmazonSQSConfig();
        var amazonSnsConfig = new AmazonSimpleNotificationServiceConfig();
        var amazonS3Config = new AmazonS3Config();
        if (!string.IsNullOrWhiteSpace(amazonsqsTransportConfig.RegionName))
        {

            amazonSqsConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(amazonsqsTransportConfig.RegionName);
            amazonSnsConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(amazonsqsTransportConfig.RegionName);
        }

        // If ServiceUrl is passed, we are assuming we are using LocalStack
        // Without this, local stack will try to really authenticate with aws which will fail
        if (!string.IsNullOrWhiteSpace(amazonsqsTransportConfig.ServiceUrl))
        {
            amazonSnsConfig.ServiceURL = amazonsqsTransportConfig.ServiceUrl;
            amazonSqsConfig.ServiceURL = amazonsqsTransportConfig.ServiceUrl;
        }

        if (amazonsqsTransportConfig.S3BucketSettings is not null)
        {
            amazonS3Config.ServiceURL = amazonsqsTransportConfig.ServiceUrl;
            amazonS3Config.RegionEndpoint = RegionEndpoint.GetBySystemName(amazonsqsTransportConfig.RegionName);
        }

        var sqsClient = new AmazonSQSClient(credentials, amazonSqsConfig);
        var snsClient = new AmazonSimpleNotificationServiceClient(credentials, amazonSnsConfig);

        var sqsTransport = new SqsTransport(sqsClient, snsClient);
        if (amazonsqsTransportConfig.S3BucketSettings is not null)
        {
            sqsTransport.S3 = new S3Settings(amazonsqsTransportConfig.S3BucketSettings.BucketName, amazonsqsTransportConfig.S3BucketSettings.KeyPrefix, new AmazonS3Client(amazonS3Config));
        }

        return new SqsTransport(sqsClient, snsClient);
    }

    private AzureServiceBusTransport CreateAzureServiceBusTransport(string transportConnectionString)
    {
        return new AzureServiceBusTransport(transportConnectionString, TopicTopology.Default);
    }

    private static async Task<RawEndpoint> InternalCreateEndpoint(
        string endpointName,
        TransportDefinition transport,
        bool setupInfrastructure = true)
    {
        var infrastructure = await InternalCreateInfrastructure(
            endpointName,
            transport,
            isReceiveEnabled: true,
            setupInfrastructure);

        return new RawEndpoint(infrastructure, endpointName);
    }

    private static async Task<RawSendOnlyEndpoint> InternalCreateSendOnlyEndpoint(
        string endpointName,
        TransportDefinition transport)
    {
        var infrastructure = await InternalCreateInfrastructure(
            endpointName,
            transport,
            isReceiveEnabled: false,
            setupInfrastructure: false);

        return new RawSendOnlyEndpoint(infrastructure, endpointName);
    }

    private static async Task<TransportInfrastructure> InternalCreateInfrastructure(
        string endpointName,
        TransportDefinition transport,
        bool isReceiveEnabled,
        bool setupInfrastructure)
    {
        var hostSettings = new HostSettings(
            endpointName,
            endpointName,
            new StartupDiagnosticEntries(),
            (message, exception, token) =>
            {
                // Console.WriteLine("Critical error: " + exception);
            },
            // Do not run the infrastructure provisioning and assume things are already present.
            setupInfrastructure);

        var infrastructure = await transport.Initialize(hostSettings,
            isReceiveEnabled
                ?
                [
                    new ReceiveSettings(
                        "Primary",
                        new QueueAddress(endpointName),
                        isReceiveEnabled,
                        false,
                        "error")
                ]
                : [], []);


        return infrastructure;
    }
}