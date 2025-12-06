using BuslyCLI.Config;
using BuslyCLI.Config.Validators;
using BuslyCLI.Factories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BuslyCLI.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBuslyCLIServices(this IServiceCollection services)
    {
        services.AddScoped<IRawEndpointFactory, RawEndpointFactory>();
        services.AddSingleton<INServiceBusConfiguration, NServiceBusConfiguration>();
        services.AddValidatorsFromAssemblyContaining<RootConfigValidator>();
        services.AddYamlDeserializer();
        services.AddYamlSerializer();

        return services;
    }

    private static IServiceCollection AddYamlDeserializer(this IServiceCollection services)
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
                        { "amazonsqs-transport-config", typeof(AmazonsqsTransportConfig) },
                        { "azure-service-bus-transport-config", typeof(AzureServiceBusTransportConfig) },
                        { "sql-server-transport-config", typeof(SqlServerTransportConfig) }
                    };

                    o.AddUniqueKeyTypeDiscriminator<ITransportConfig>(keyMappings);
                })
                .Build());
        return services;
    }

    private static IServiceCollection AddYamlSerializer(this IServiceCollection services)
    {
        services.AddSingleton(
            new SerializerBuilder()
                .WithIndentedSequences()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build());
        return services;
    }
}