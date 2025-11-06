using Testcontainers.ServiceBus;

namespace NServiceBusCLI.Console.Tests.EndToEnd.AzureServiceBus;

public class AzureServiceBusEndToEndTestBase : EndToEnd.SingletonTestFixtureBase<ServiceBusContainer>
{
    protected ServiceBusContainer ServiceBusContainer => Container;

    protected IList<Tuple<string, string>> GeneratedTestEndpointNamesAndSubscribedEvent =
        new List<Tuple<string, string>>();


    protected override ServiceBusContainer CreateContainer()
    {
        var azureEmulatorConfigFile = CreateAzureEmulatorConfigFile();
        var emulatorConfigFilePath = Path.GetTempFileName();
        File.WriteAllText(emulatorConfigFilePath, azureEmulatorConfigFile);
        return new ServiceBusBuilder()
            .WithImage("mcr.microsoft.com/azure-messaging/servicebus-emulator:latest")
            .WithAcceptLicenseAgreement(true)
            // .WithConfig("./EndToEnd/AzureServiceBus/azure-emulator-config.json")
            .WithConfig(emulatorConfigFilePath)
            .Build();
    }

    private string CreateAzureEmulatorConfigFile()
    {
        for (var i = 0; i < 20; i++)
        {
            GeneratedTestEndpointNamesAndSubscribedEvent.Add(new Tuple<string, string>(
                $"TestEndpoint-{Guid.NewGuid():N}", $"MessageContracts.Events.OrderCreated-{Guid.NewGuid():N}"));
        }

        // Build queue entries as JSON strings
        var queueEntries = GeneratedTestEndpointNamesAndSubscribedEvent.Select(endpoint => $@"{{
                ""Name"": ""{endpoint.Item1}"",
                ""Properties"": {{
                    ""DeadLetteringOnMessageExpiration"": false,
                    ""DefaultMessageTimeToLive"": ""PT1H"",
                    ""LockDuration"": ""PT1M"",
                    ""MaxDeliveryCount"": 10,
                    ""RequiresDuplicateDetection"": false,
                    ""RequiresSession"": false
                }}
            }}")
            .ToList();

        // Join all queue entries into the queues array
        string queuesJson = string.Join(",\n", queueEntries);

        // Create Subscriptions to the event
        var topicEntries = GeneratedTestEndpointNamesAndSubscribedEvent.Select(endpoint => $@"{{
                            ""Name"": ""{endpoint.Item2}"",
                            ""Properties"": {{
                              ""DefaultMessageTimeToLive"": ""PT1H"",
                              ""DuplicateDetectionHistoryTimeWindow"": ""PT20S"",
                              ""RequiresDuplicateDetection"": false
                            }},
                            ""Subscriptions"": [
                                {{
                                ""Name"": ""{endpoint.Item1}"",
                                ""Properties"": {{
                                  ""DeadLetteringOnMessageExpiration"": false,
                                  ""DefaultMessageTimeToLive"": ""PT1H"",
                                  ""LockDuration"": ""PT1M"",
                                  ""MaxDeliveryCount"": 3,
                                  ""ForwardDeadLetteredMessagesTo"": """",
                                  ""ForwardTo"": ""{endpoint.Item1}"",
                                  ""RequiresSession"": false
                                }},
                                ""Rules"": [
                                  {{
                                    ""Name"": ""$default"",
                                    ""Properties"": {{
                                      ""FilterType"": ""Sql"",
                                      ""SqlFilter"": {{
                                        ""SqlExpression"": ""1=1""
                                      }}
                                    }}
                                  }}
                                ]
                              }}
                            ]
                          }}")
            .ToList();

        string topicsJson = string.Join(",\n", topicEntries);

        // Final config string
        string emulatorConfig = $@"{{
            ""UserConfig"": {{
                ""Namespaces"": [
                    {{
                        ""Name"": ""sbemulatorns"",
                        ""Queues"": [
                            {queuesJson}
                        ],
                        ""Topics"": [
                         {topicsJson}
                        ]
                    }}
                ],
                ""Logging"": {{
                    ""Type"": ""File""
                }}
            }}
        }}";
        // string emulatorConfig = $@"{{
        //     ""UserConfig"": {{
        //         ""Namespaces"": [
        //             {{
        //                 ""Name"": ""sbemulatorns"",
        //                 ""Queues"": [
        //                     {queuesJson}
        //                 ],
        //                 ""Topics"": [
        //                   {{
        //                     ""Name"": ""MessageContracts.Events.OrderCreated"",
        //                     ""Properties"": {{
        //                       ""DefaultMessageTimeToLive"": ""PT1H"",
        //                       ""DuplicateDetectionHistoryTimeWindow"": ""PT20S"",
        //                       ""RequiresDuplicateDetection"": false
        //                     }},
        //                     ""Subscriptions"": [
        //                         {subscriptionsJson}
        //                     ]
        //                   }}
        //                 ]
        //             }}
        //         ],
        //         ""Logging"": {{
        //             ""Type"": ""File""
        //         }}
        //     }}
        // }}";

        return emulatorConfig;
    }

    protected override async Task StartContainerAsync(ServiceBusContainer container)
    {
        await container.StartAsync();
    }
}