using Microsoft.Extensions.DependencyInjection;
using NServiceBusCLI.Config;
using NServiceBusCLI.Factories;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NServiceBusCLI.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNServiceBusCliServices(this IServiceCollection services)
    {
        services.AddScoped<IRawEndpointFactory, RawEndpointFactory>();

        return services;
    }

    public static IServiceCollection AddYamlDeserializer(this IServiceCollection services)
    {
        services.AddSingleton(
            new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .WithTypeDiscriminatingNodeDeserializer((o) =>
                {
                    var keyMappings = new Dictionary<string, Type>
                    {
                        { "learning-transport-config", typeof(LearningTransportConfig) },
                        { "rabbitmq-transport-config", typeof(RabbitmqTransportConfig) },
                        { "amazonsqs-transport-config", typeof(AmazonsqsTransportConfig) }
                    };

                    o.AddUniqueKeyTypeDiscriminator<ITransportConfig>(keyMappings);
                })
                .Build());
        return services;
    }

    public static IServiceCollection AddYamlSerializer(this IServiceCollection services)
    {
        services.AddSingleton(
            new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) // Optional
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build());
        return services;
    }
}