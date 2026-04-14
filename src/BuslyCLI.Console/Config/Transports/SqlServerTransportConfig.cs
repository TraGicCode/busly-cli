namespace BuslyCLI.Config.Transports;

public class SqlServerTransportConfig : ITransportConfig
{
    public string ConnectionString { get; set; }
}