using BuslyCLI.Commands.ServiceControl.Message;

namespace BuslyCLI.Console.Tests.Commands.ServiceControl.Message;

[TestFixture]
public class SearchMessagesSettingsTests
{
    [Test]
    public void ShouldFailWhenToIsPassedWithoutFrom()
    {
        var settings = new SearchMessagesSettings
        {
            To = DateTime.UtcNow
        };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.False);
        Assert.That(result.Message, Does.Contain("Both --from and --to must be provided together."));
    }

    [Test]
    public void ShouldFailWhenFromPassedWithoutTo()
    {
        var settings = new SearchMessagesSettings
        {
            To = DateTime.UtcNow
        };

        var result = settings.Validate();

        Assert.That(result.Successful, Is.False);
        Assert.That(result.Message, Does.Contain("Both --from and --to must be provided together."));
    }
}