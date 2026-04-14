using BuslyCLI.Config.Transports;
using BuslyCLI.Config.Validators;
using FluentValidation.TestHelper;

namespace BuslyCLI.Console.Tests.Config.Validators;

[TestFixture]
public class AzureStorageQueuesTransportConfigValidatorTests
{
    private readonly AzureStorageQueuesTransportConfigValidator _validator;

    public AzureStorageQueuesTransportConfigValidatorTests()
    {
        _validator = new AzureStorageQueuesTransportConfigValidator();
    }

    [Test]
    public async Task ShouldErrorWhenConnectionStringIsNotPassed()
    {
        // Arrange
        var azureStorageQueuesTransportConfig = new AzureStorageQueuesTransportConfig
        {
            ConnectionString = null
        };
        // Act
        var result = await _validator.TestValidateAsync(azureStorageQueuesTransportConfig);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ConnectionString)
            .WithErrorMessage("'Connection String' must not be empty.");
    }

    [Test]
    public async Task ShouldNotErrorWhenConnectionStringIsPassed()
    {
        // Arrange
        var azureStorageQueuesTransportConfig = new AzureStorageQueuesTransportConfig
        {
            ConnectionString = "DefaultEndpointsProtocol=https;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1"
        };
        // Act
        var result = await _validator.TestValidateAsync(azureStorageQueuesTransportConfig);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.ConnectionString);
    }
}