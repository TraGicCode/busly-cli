using BuslyCLI.Config;
using TransportConfig = BuslyCLI.Config.TransportConfig;

namespace BuslyCLI.Console.Tests.EndToEnd.Learning;

public abstract class LearningEndToEndTestBase : EndToEndTestBase
{
    protected override TransportConfig CreateTransportConfig() => new()
    {
        LearningTransportConfig = new LearningTransportConfig
        {
            StorageDirectory = "./.learningtransport"
        }
    };
}