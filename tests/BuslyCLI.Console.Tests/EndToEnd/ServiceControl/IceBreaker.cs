using System.Net;
using System.Text.Json;
using BuslyCLI.Console.Tests.Commands;
using BuslyCLI.Console.Tests.TestHelpers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Spectre.Console.Testing;
using Testcontainers.RabbitMq;

namespace BuslyCLI.Console.Tests.EndToEnd.ServiceControl;

[TestFixture]
[Ignore("Not refactored and completed yet")]
public class IceBreakerTest : CommandTestBase
{
    protected readonly JsonSerializerOptions _jsonObjectOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };


    protected IContainer serviceControl;
    protected IContainer serviceControlAudit;
    protected IContainer serviceControlMonitoring;
    protected IContainer servicePulse;
    protected IContainer serviceControlDb;
    protected RabbitMqContainer rabbitMq;

    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        // await InitializeContainer();
        var network = new NetworkBuilder()
            .Build();

        rabbitMq = new RabbitMqBuilder("rabbitmq:3-management")
            .WithNetwork(network)
            .WithPortBinding(15672, true)
            .WithPortBinding(5672, true)
            .WithPassword("guest")
            .WithUsername("guest")
            .WithNetworkAliases("rabbitmq")
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilInternalTcpPortIsAvailable(5672)
                    .UntilInternalTcpPortIsAvailable(15672)
                    .UntilCommandIsCompleted("rabbitmq-diagnostics check_port_connectivity"))
            .Build();

        await rabbitMq.StartAsync();


        serviceControlDb = new ContainerBuilder("particular/servicecontrol-ravendb")
            .WithName("servicecontrol-db")
            .WithNetwork(network)
            .WithNetworkAliases("servicecontrol-db")
            .WithEnvironment("TRANSPORTTYPE", "RabbitMQ.QuorumConventionalRouting")
            .WithEnvironment("CONNECTIONSTRING", "host=rabbitmq;username=guest;password=guest")
            .WithEnvironment("RAVENDB_CONNECTIONSTRING", "http://servicecontrol-db:8080")
            .WithEnvironment("REMOTEINSTANCES", """[{"api_uri":"http://audit:44444/api"}]""")
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilInternalTcpPortIsAvailable(8080))
            .Build();

        serviceControl = new ContainerBuilder("particular/servicecontrol")
            .WithName("servicecontrol")
            .WithNetwork(network)
            .WithNetworkAliases("servicecontrol")
            .WithPortBinding(33333, true)
            .WithEnvironment("TRANSPORTTYPE", "RabbitMQ.QuorumConventionalRouting")
            .WithEnvironment("CONNECTIONSTRING", "host=rabbitmq;username=guest;password=guest")
            .WithEnvironment("RAVENDB_CONNECTIONSTRING", "http://servicecontrol-db:8080")
            .WithEnvironment("REMOTEINSTANCES", "[{\"api_uri\":\"http://audit:44444/api\"}]")
            .WithCommand("--setup-and-run")
            .DependsOn(serviceControlDb)
            .DependsOn(rabbitMq)
            // .WithWaitStrategy(
            //     Wait.ForUnixContainer()
            //         .UntilInternalTcpPortIsAvailable(33333))
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r => r
                            .ForPort(33333)
                            .ForPath("/api")
                            .ForStatusCode(HttpStatusCode.OK),
                        o => o
                            .WithRetries(20)
                            .WithInterval(TimeSpan.FromSeconds(5))
                            .WithTimeout(TimeSpan.FromSeconds(45))))
            .Build();

        serviceControlAudit = new ContainerBuilder("particular/servicecontrol-audit")
            .WithName("servicecontrol-audit")
            .WithNetwork(network)
            .WithNetworkAliases("servicecontrol-audit")
            .WithPortBinding(44444, true)
            .WithEnvironment("TRANSPORTTYPE", "RabbitMQ.QuorumConventionalRouting")
            .WithEnvironment("CONNECTIONSTRING", "host=rabbitmq;username=guest;password=guest")
            .WithEnvironment("RAVENDB_CONNECTIONSTRING", "http://servicecontrol-db:8080")
            .WithEnvironment("SERVICECONTROLQUEUEADDRESS", "Particular.ServiceControl")
            .WithCommand("--setup-and-run")
            .DependsOn(serviceControlDb)
            .DependsOn(rabbitMq)
            // .WithWaitStrategy(
            //     Wait.ForUnixContainer()
            //         .UntilInternalTcpPortIsAvailable(44444))
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r => r
                            .ForPort(44444)
                            .ForPath("/api")
                            .ForStatusCode(HttpStatusCode.OK),
                        o => o
                            .WithRetries(20)
                            .WithInterval(TimeSpan.FromSeconds(5))
                            .WithTimeout(TimeSpan.FromSeconds(45))))
            .Build();

        serviceControlMonitoring = new ContainerBuilder("particular/servicecontrol-monitoring")
            .WithName("servicecontrol-monitoring")
            .WithNetwork(network)
            .WithNetworkAliases("servicecontrol-monitoring")
            .WithPortBinding(33633, true)
            .WithEnvironment("TRANSPORTTYPE", "RabbitMQ.QuorumConventionalRouting")
            .WithEnvironment("CONNECTIONSTRING", "host=rabbitmq;username=guest;password=guest")
            .WithCommand("--setup-and-run")
            .DependsOn(rabbitMq)
            // .WithWaitStrategy(
            //     Wait.ForUnixContainer()
            //         .UntilInternalTcpPortIsAvailable(33633))
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r => r
                            .ForPort(33633)
                            .ForPath("/")
                            .ForStatusCode(HttpStatusCode.OK),
                        o => o
                            .WithRetries(20)
                            .WithInterval(TimeSpan.FromSeconds(5))
                            .WithTimeout(TimeSpan.FromSeconds(45))))
            .Build();

        servicePulse = new ContainerBuilder("particular/servicepulse")
            .WithName("servicepulse")
            .WithNetwork(network)
            .WithNetworkAliases("servicepulse")
            .WithPortBinding(9090, true)
            .WithEnvironment("SERVICECONTROL_URL", "http://servicecontrol:33333")
            .WithEnvironment("MONITORING_URL", "http://servicecontrol-monitoring:33633")
            .DependsOn(rabbitMq)
            .DependsOn(serviceControl)
            .DependsOn(serviceControlMonitoring)
            // .WithWaitStrategy(
            //     Wait.ForUnixContainer()
            //         .UntilInternalTcpPortIsAvailable(9090))
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r => r
                            .ForPort(9090)
                            .ForPath("/")
                            .ForStatusCode(HttpStatusCode.OK),
                        o => o
                            .WithRetries(20)
                            .WithInterval(TimeSpan.FromSeconds(5))
                            .WithTimeout(TimeSpan.FromSeconds(45))))
            .Build();

        await serviceControlDb.StartAsync();

        await serviceControl.StartAsync();
        await serviceControlAudit.StartAsync();
        await serviceControlMonitoring.StartAsync();
        await servicePulse.StartAsync();

        // Create an endpoint that can send heartbeats
        var endpointConfiguration = new EndpointConfiguration("EndpointWithHeartBeats");
        //
        // Transport
        //
        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.ConnectionString(rabbitMq.GetConnectionString());
        transport.ManagementApiConfiguration($"http://{rabbitMq.Hostname}:{rabbitMq.GetMappedPublicPort(15672)}");
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl", TimeSpan.FromSeconds(1));
        var endpointInstance = await Endpoint.Start(endpointConfiguration);
    }

    [OneTimeTearDown]
    public virtual async Task OneTimeTearDown()
    {
        await servicePulse.DisposeAsync();
        await serviceControlMonitoring.DisposeAsync();
        await serviceControl.DisposeAsync();
        await serviceControlAudit.DisposeAsync();
        await rabbitMq.DisposeAsync();
        await serviceControlDb.DisposeAsync();

    }

    [Test]
    public void ShouldShowLicense()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };
        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        // TODO: this shouldn't blow up if a transport isn't set. Fix this validation later
        var yamlFile = $"""
                        ---
                        current-service-control-instance: local-service-control
                        service-control-instances:
                          - name: local-service-control
                            url: http://localhost:{serviceControl.GetMappedPublicPort(33333)}/api
                        """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run(
            "sc",
            "license",
            "show",
            "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
        Assert.That(result.Output, Is.EqualTo(
            """
                ┌────────┬───────────────┬──────────┬─────────┬──────────────────────┐
                │ Status │ Registered To │ Is Trial │ Edition │ Expiration Date      │
                ├────────┼───────────────┼──────────┼─────────┼──────────────────────┤
                │ Valid  │ Blinds.com    │ No       │ Premium │ 2026-05-23 00:00:00Z │
                └────────┴───────────────┴──────────┴─────────┴──────────────────────┘
                """.NormalizeLineEndings()));
    }

    [Test]
    public void ShouldListEndpoints()
    {
        // Arrange
        var messageBody = new { OrderNumber = Guid.NewGuid() };
        var json = JsonSerializer.Serialize(messageBody, _jsonObjectOptions);
        // TODO: this shouldn't blow up if a transport isn't set. Fix this validation later
        var yamlFile = $"""
                        ---
                        current-service-control-instance: local-service-control
                        service-control-instances:
                          - name: local-service-control
                            url: http://localhost:{serviceControl.GetMappedPublicPort(33333)}/api
                        """;
        using var configFile = new TestableNServiceBusConfigurationFile(yamlFile);

        // Act
        var result = Sut.Run(
            "sc",
            "endpoints",
            "list",
            "--config", configFile.FilePath);

        // Assert
        Assert.That(result.ExitCode, Is.EqualTo(0));
    }
}