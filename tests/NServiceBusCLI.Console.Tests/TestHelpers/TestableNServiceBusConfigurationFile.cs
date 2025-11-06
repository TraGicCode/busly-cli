namespace NServiceBusCLI.Console.Tests.TestHelpers;
using System;
using System.IO;

public class TestableNServiceBusConfigurationFile : IDisposable
{
    public string FilePath { get; private set; }

    public TestableNServiceBusConfigurationFile(string fileContent)
    {
        FilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".yaml");
        using (File.Create(FilePath)) { } // Create the file
        File.WriteAllText(FilePath, fileContent);
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
        catch
        {
            // Suppress any exception on dispose to avoid crashing during GC finalization
        }
    }
}