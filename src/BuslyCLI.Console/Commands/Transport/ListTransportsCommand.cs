using BuslyCLI.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BuslyCLI.Commands.Transport;

// Config file POC
// ~/.busly-cli/config.yaml
//
// Contents
//
// transports:
// 	- name: local-learning
//    learning-transport-config:
//       	storage-directory: .learningtransport
//     - name: local-rabbitmq
//       rabbitmq-transport-config:
//         amqp-connection-string:

public class ListTransportsCommand(IAnsiConsole console, INServiceBusConfiguration nservicebusConfiguration)
    : AsyncCommand<ListTransportsSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ListTransportsSettings settings, CancellationToken cancellationToken)
    {
        var grid = new Grid();

        // Add columns
        grid.AddColumn();
        grid.AddColumn();
        grid.AddColumn();

        // Add header row
        grid.AddRow("CURRENT", "NAME", "TRANSPORT-TYPE");

        var nsbConfiguration = await nservicebusConfiguration.GetUnValidatedConfigurationAsync(settings.Config.Path);

        if (nsbConfiguration is { Transports: not null })
        {
            foreach (var transport in nsbConfiguration.Transports)
            {
                grid.AddRow(nsbConfiguration.CurrentTransport == transport.Name ? "*" : "", transport.Name,
                    TransportConfigTypeToString(transport.Config));
            }
        }

        console.Write(grid);
        return 0;
    }

    public string TransportConfigTypeToString(ITransportConfig transportConfig)
    {
        switch (transportConfig)
        {
            case RabbitmqTransportConfig rabbitmqConfig:
                return "rabbitmq";
            case LearningTransportConfig learningConfig:
                return "learning";
            case AzureServiceBusTransportConfig azureServiceBusConfig:
                return "azure-service-bus";
            case AzureStorageQueuesTransportConfig azureStorageQueuesTransportConfig:
                return "azure-storage-queues";
            case AmazonsqsTransportConfig amazonsqsTransportConfig:
                return "amazon-sqs";
            case SqlServerTransportConfig sqlServerTransportConfig:
                return "sql-server";
            case PostgreSqlTransportConfig postgreSqlTransportConfig:
                return "postgre-sql";
            default:
                throw new ApplicationException("Unknown transport type");
        }
    }
}