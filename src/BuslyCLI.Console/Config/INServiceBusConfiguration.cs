using FluentValidation;
using YamlDotNet.Serialization;

namespace BuslyCLI.Config;

public interface INServiceBusConfiguration
{
    Task<NServiceBusConfig> GetValidatedConfigurationAsync(string path);

    Task<NServiceBusConfig> GetUnValidatedConfigurationAsync(string path);

    Task PersistConfiguration(string path, NServiceBusConfig config);
}

public class NServiceBusConfiguration(IDeserializer yamlDeserializer, ISerializer yamlSerializer, IValidator<NServiceBusConfig> validator) : INServiceBusConfiguration
{


    private async Task<NServiceBusConfig> LoadConfigurationAsync(
        string path,
        bool validate)
    {
        if (!File.Exists(path)) return null;

        var yaml = await File.ReadAllTextAsync(path);
        var config = yamlDeserializer.Deserialize<NServiceBusConfig>(yaml);

        // config is null if yaml file is empty
        if (config is null) return null;

        if (validate)
            await validator.ValidateAsync(config, opts => opts.ThrowOnFailures());

        return config;
    }

    public async Task<NServiceBusConfig> GetValidatedConfigurationAsync(string path)
        => await LoadConfigurationAsync(path, validate: true);

    public async Task<NServiceBusConfig> GetUnValidatedConfigurationAsync(string path)
        => await LoadConfigurationAsync(path, validate: false);

    public async Task PersistConfiguration(string path, NServiceBusConfig config)
    {
        var yaml = yamlSerializer.Serialize(config);
        await File.WriteAllTextAsync(path, yaml);
    }
}