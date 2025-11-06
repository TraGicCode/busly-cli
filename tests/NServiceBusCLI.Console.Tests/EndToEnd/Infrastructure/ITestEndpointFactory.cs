using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using NServiceBus.Transport;

namespace NServiceBusCLI.Console.Tests.EndToEnd.Infrastructure;

public class TestEndpointFactory
{
    /// <summary>
    ///     Generates a unique endpoint name for testing purposes.
    /// </summary>
    /// <param name="prefix">Optional prefix for the endpoint name. Defaults to "TestEndpoint".</param>
    /// <returns>A unique endpoint name with the format "{prefix}-{guid}"</returns>
    public static string GenerateUniqueEndpointName(string prefix = "TestEndpoint")
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }

    public async Task<TestEndpoint> CreateRabbitMQTestEndpoint(string transportConnectionString,
        string managementApiUrl)
    {
        var name = GenerateUniqueEndpointName();
        var transport = new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), transportConnectionString)
        {
            ManagementApiConfiguration = new ManagementApiConfiguration(managementApiUrl)
        };
        return await InternalCreateTestEndpoint(name, transport);
    }

    public async Task<TestEndpoint> CreateAmazonSQSTestEndpoint(string transportConnectionString)
    {
        var name = GenerateUniqueEndpointName();

        // Set up AWS credentials and region
        var credentials = new BasicAWSCredentials("test", "test");

        var sqsClient = new AmazonSQSClient(credentials, new AmazonSQSConfig
        {
            ServiceURL = transportConnectionString,
            AuthenticationRegion = "us-east-1",
        });

        var snsClient = new AmazonSimpleNotificationServiceClient(credentials, new AmazonSimpleNotificationServiceConfig
        {
            ServiceURL = transportConnectionString,
            AuthenticationRegion = "us-east-1",
        });

        return await InternalCreateTestEndpoint(name, new SqsTransport(sqsClient, snsClient));
    }

    public async Task<TestEndpoint> CreateLearningTestEndpoint(string storageDirectory)
    {
        var name = GenerateUniqueEndpointName();
        return await InternalCreateTestEndpoint(name, new LearningTransport()
        {
            StorageDirectory = storageDirectory
        });
    }

    public async Task<TestEndpoint> CreateAzureServiceBusTestEndpoint(string endpointName, string connectionString)
    {
        return await InternalCreateTestEndpoint(endpointName, new AzureServiceBusTransport(connectionString, TopicTopology.Default));
    }

    private static async Task<TestEndpoint> InternalCreateTestEndpoint(string endpointName,
        TransportDefinition transport)
    {
        var hostSettings = new HostSettings(
            endpointName,
            endpointName,
            new StartupDiagnosticEntries(),
            criticalErrorAction: (message, exception, token) =>
            {
                TestContext.Out.WriteLine("Critical error: " + exception);
            },
            // TODO: This needs to be false for "Azure Service Bus Emulator" tests to pass
            transport is not AzureServiceBusTransport);

        var infrastructure = await transport.Initialize(hostSettings, new[]
        {
            new ReceiveSettings(
                "Primary",
                new QueueAddress(endpointName),
                true,
                false,
                "error")
        }, new string[0]);

        return new TestEndpoint(infrastructure);
    }
}