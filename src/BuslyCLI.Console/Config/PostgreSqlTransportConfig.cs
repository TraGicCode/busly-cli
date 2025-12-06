namespace BuslyCLI.Config;

public class PostgreSqlTransportConfig : ITransportConfig
{
    public string ConnectionString { get; set; }
}