namespace BuslyCLI.Console.Tests;

public static class TestEndpointNameGenerator
{
    public static string GenerateUniqueEndpointName(string prefix = "TestEndpoint")
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }
}