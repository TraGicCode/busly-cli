namespace BuslyCLI.Config.Transports;

public class PostgreSqlTransportConfig : ITransportConfig
{
    public string ConnectionString { get; set; }
}