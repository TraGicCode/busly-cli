using BuslyCLI.Commands.NsbEvent;

namespace BuslyCLI.Console.Tests.Commands;

[TestFixture]
public class CommonMessageSettingsTests
{
    [Test]
    public void ShouldFailWhenContentTypeIsMissing()
    {
        var settings = new PublishCommandSettings
        {
            ContentType = null!,
            EnclosedMessageType = "MessageContracts.Commands.CreateOrder",
            MessageBody = "{}"
        };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.False);
        Assert.That(result.Message, Does.Contain("must specify a 'content-type'."));
    }

    [Test]
    public void ShouldFailWhenEnclosedMessageTypeIsMissing()
    {
        var settings = new PublishCommandSettings
        {
            ContentType = "application/json",
            EnclosedMessageType = null!,
            MessageBody = "{}"
        };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.False);
        Assert.That(result.Message, Does.Contain("must specify an 'enclosed-message-type'."));
    }

    [Test]
    public void ShouldFailWhenMessageBodyIsMissing()
    {
        var settings = new PublishCommandSettings
        {
            ContentType = "application/json",
            EnclosedMessageType = "MessageContracts.Commands.CreateOrder",
            MessageBody = null!
        };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.False);
        Assert.That(result.Message, Does.Contain("must specify a 'message-body'."));
    }
}