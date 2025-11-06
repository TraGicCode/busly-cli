using YamlDotNet.Serialization;

namespace NServiceBusCLI.Config;

public interface INServiceBusConfiguration
{
    Task<NServiceBusConfig> GetConfigurationAsync(string path);

    Task PersistConfiguration(string path, NServiceBusConfig config);
}

public class NServiceBusConfiguration(IDeserializer yamlDeserializer, ISerializer yamlSerializer) : INServiceBusConfiguration
{

    public async Task<NServiceBusConfig> GetConfigurationAsync(string path)
    {
        if (File.Exists(path))
        {
            var yaml = await File.ReadAllTextAsync(path);
            return yamlDeserializer.Deserialize<NServiceBusConfig>(yaml);
        }

        return null;
    }

    public async Task PersistConfiguration(string path, NServiceBusConfig config)
    {
        var yaml = yamlSerializer.Serialize(config);
        await File.WriteAllTextAsync(path, yaml);
    }
}