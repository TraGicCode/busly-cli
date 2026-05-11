using BuslyCLI.Config.Validators;
using FluentValidation;
using YamlDotNet.Serialization;

namespace BuslyCLI.Config;

public interface INServiceBusConfiguration
{
    Task<NServiceBusConfig> GetTransportValidatedConfigurationAsync(string path);

    Task<NServiceBusConfig> GetServiceControlValidatedConfigurationAsync(string path);

    Task<NServiceBusConfig> GetUnValidatedConfigurationAsync(string path);

    Task PersistConfiguration(string path, NServiceBusConfig config);
    Task UpdateCurrentTransportAsync(string path, string newTransport);
    Task RemoveTransportAsync(string path, string transportToRemove);
    Task UpdateCurrentServiceControlInstanceAsync(string path, string newInstance);
    Task RemoveServiceControlInstanceAsync(string path, string instanceToRemove);
}

public class NServiceBusConfiguration(
    IDeserializer yamlDeserializer,
    ISerializer yamlSerializer) : INServiceBusConfiguration
{

    private async Task<NServiceBusConfig> LoadConfigurationAsync(
        string path,
        bool validate,
        IValidator<NServiceBusConfig> overrideValidator = null)
    {
        if (!File.Exists(path)) return null;

        var yaml = await File.ReadAllTextAsync(path);
        var config = yamlDeserializer.Deserialize<NServiceBusConfig>(yaml);

        // config is null if yaml file is empty
        if (config is null) return null;

        if (validate && overrideValidator is not null)
            await overrideValidator.ValidateAsync(config, opts => opts.ThrowOnFailures());

        return config;
    }

    public async Task<NServiceBusConfig> GetTransportValidatedConfigurationAsync(string path)
        => await LoadConfigurationAsync(path, validate: true, overrideValidator: new TransportOnlyConfigValidator());

    public async Task<NServiceBusConfig> GetServiceControlValidatedConfigurationAsync(string path)
        => await LoadConfigurationAsync(path, validate: true, overrideValidator: new ServiceControlOnlyConfigValidator());

    public async Task<NServiceBusConfig> GetUnValidatedConfigurationAsync(string path)
        => await LoadConfigurationAsync(path, validate: false);

    public async Task PersistConfiguration(string path, NServiceBusConfig config)
    {
        var yaml = yamlSerializer.Serialize(config);
        await File.WriteAllTextAsync(path, yaml);
    }

    public async Task UpdateCurrentTransportAsync(string path, string newTransport)
    {
        // Load the YAML
        var lines = await File.ReadAllLinesAsync(path);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimStart();

            if (line.StartsWith("current-transport:"))
            {
                int indent = lines[i].Length - line.Length; // preserve original indent

                lines[i] =
                    new string(' ', indent) +
                    "current-transport: " + newTransport;

                break;
            }
        }

        await File.WriteAllLinesAsync(path, lines);
    }

    public async Task RemoveTransportAsync(string path, string transportToRemove)
    {
        var lines = (await File.ReadAllLinesAsync(path)).ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].TrimStart().StartsWith($"- name: {transportToRemove}"))
            {
                int indent = lines[i].TakeWhile(char.IsWhiteSpace).Count();
                int j = i + 1;

                // Continue deleting child lines until indentation decreases
                while (j < lines.Count &&
                       lines[j].TakeWhile(char.IsWhiteSpace).Count() > indent)
                {
                    j++;
                }

                // Remove the array item block
                lines.RemoveRange(i, j - i);
                break;
            }
        }

        await File.WriteAllLinesAsync(path, lines);
    }

    public async Task UpdateCurrentServiceControlInstanceAsync(string path, string newInstance)
    {
        var lines = await File.ReadAllLinesAsync(path);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimStart();

            if (line.StartsWith("current-service-control-instance:"))
            {
                int indent = lines[i].Length - line.Length;
                lines[i] = new string(' ', indent) + "current-service-control-instance: " + newInstance;
                break;
            }
        }

        await File.WriteAllLinesAsync(path, lines);
    }

    public async Task RemoveServiceControlInstanceAsync(string path, string instanceToRemove)
    {
        var lines = (await File.ReadAllLinesAsync(path)).ToList();

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].TrimStart().StartsWith($"- name: {instanceToRemove}"))
            {
                int indent = lines[i].TakeWhile(char.IsWhiteSpace).Count();
                int j = i + 1;

                while (j < lines.Count &&
                       lines[j].TakeWhile(char.IsWhiteSpace).Count() > indent)
                {
                    j++;
                }

                lines.RemoveRange(i, j - i);
                break;
            }
        }

        await File.WriteAllLinesAsync(path, lines);
    }
}