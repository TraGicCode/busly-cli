namespace BuslyCLI.Console.Tests.TestHelpers;

public static class TestEndpointNameGenerator
{
    public static string GenerateUniqueEndpointName(string prefix = "TestEndpoint")
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }
}