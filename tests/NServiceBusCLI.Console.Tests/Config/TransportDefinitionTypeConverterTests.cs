using NServiceBusCLI.Config;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NServiceBusCLI.Console.Tests.Config;

[TestFixture]
public class TransportDefinitionTypeConverterTests
{
    [Test]
    public void CanConvertToExpectedTypes()
    {
        var yamlFile = """
                       ---
                       current-transport: local-learning
                       transports:
                         - name: local-learning
                           learning-transport-config:
                             storage-directory: .learningtransport
                         - name: local-rabbitmq
                           rabbitmq-transport-config:
                             amqp-connection-string: amqp://localhost
                         - name: local-amazonsqs
                           amazonsqs-transport-config:
                             service-url: http://localhost:4566
                         - name: local-azure-service-bus
                           azure-service-bus-transport-config:
                             connection-string: Endpoint=amqp://127.0.0.1:32799/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true
                       """;

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .WithTypeDiscriminatingNodeDeserializer((o) =>
            {
                var keyMappings = new Dictionary<string, Type>
                {
                    { "learning-transport-config", typeof(LearningTransportConfig) },
                    { "rabbitmq-transport-config", typeof(RabbitmqTransportConfig) },
                    { "amazonsqs-transport-config", typeof(AmazonsqsTransportConfig) },
                    { "azure-service-bus-transport-config", typeof(AzureServiceBusTransportConfig) }
                };

                o.AddUniqueKeyTypeDiscriminator<ITransportConfig>(keyMappings);
            })
            .Build();

        // Act
        var response = deserializer.Deserialize<NServiceBusConfig>(yamlFile);

        // Assert
        Assert.That(response.Transports.Count, Is.EqualTo(4));
        Assert.That(response.Transports
                .Select(t => t.Config.GetType())
                .OrderBy(t => t.Name)
                .ToArray(),
            Is.EqualTo(new[] { typeof(AmazonsqsTransportConfig), typeof(AzureServiceBusTransportConfig), typeof(LearningTransportConfig), typeof(RabbitmqTransportConfig) }));
    }

    [Test]
    public void CanSerializeToExpectedTypes()
    {
        var nservicebusConfig = new NServiceBusConfig
        {
            CurrentTransport = "local-learning",
            Transports = new List<TransportConfig>
            {
                new()
                {
                    Name = "local-learning",
                    LearningTransportConfig = new LearningTransportConfig
                    {
                        StorageDirectory = ".learningtransport"
                    }
                }
            }
        };

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) // Optional
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();

        string yaml = serializer.Serialize(nservicebusConfig);


    }
}