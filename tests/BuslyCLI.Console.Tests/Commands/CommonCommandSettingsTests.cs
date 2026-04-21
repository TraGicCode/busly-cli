using BuslyCLI.Commands.NsbCommand;
using BuslyCLI.TypeConverters;

namespace BuslyCLI.Console.Tests.Commands;

[TestFixture]
public class CommonCommandSettingsTests
{
    [Test]
    public void ShouldFailWhenDestinationEndpointIsMissing()
    {
        var settings = new SendCommandSettings
        {
            ContentType = "application/json",
            EnclosedMessageType = "MessageContracts.Commands.CreateOrder",
            MessageBody = new MessageBodyValue("{}"),
            DestinationEndpoint = null!
        };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.False);
        Assert.That(result.Message, Does.Contain("must specify a 'destination-endpoint'."));
    }
}