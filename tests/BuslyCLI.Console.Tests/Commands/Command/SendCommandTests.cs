// using Microsoft.Extensions.DependencyInjection;
// using BuslyCLI.DependencyInjection;
// using BuslyCLI.Spectre;
// using Spectre.Console.Cli.Extensions.DependencyInjection;
// using Spectre.Console.Testing;
//
// namespace BuslyCLI.Console.Tests.Commands.Command;
//
// public class SendCommandTests
// {
//     private CommandAppTester _sut;
//
//     [SetUp]
//     public void Setup()
//     {
//         var registrations = new ServiceCollection();
//         registrations.AddBuslyCLIServices();
//         using var registrar = new DependencyInjectionRegistrar(registrations);
//         _sut = new CommandAppTester(registrar);
//         _sut.Configure(AppConfiguration.GetSpectreCommandConfiguration());
//     }
//
//     [TestCase("Error: must specify a 'content-type'.", "command", "send")]
//     [TestCase("Error: Option 'content-type' is defined but no value has been provided.", "command", "send", "--content-type")]
//     [TestCase("Error: must specify an 'enclosed-message-type'.", "command", "send", "--content-type", "application/json")]
//     [TestCase("Error: Option 'enclosed-message-type' is defined but no value has been provided.", "command", "send", "--content-type", "application/json", "--enclosed-message-type")]
//     [TestCase("Error: must specify a 'message-body'.", "command", "send", "--content-type", "application/json", "--enclosed-message-type", "MessageContracts.Commands.CreateOrder")]
//     [TestCase("Error: Option 'message-body' is defined but no value has been provided.", "command", "send", "--content-type", "application/json", "--enclosed-message-type", "MessageContracts.Commands.CreateOrder", "--message-body")]
//     [TestCase("Error: must specify a 'destination-endpoint'.", "command", "send", "--content-type", "application/json", "--enclosed-message-type", "MessageContracts.Commands.CreateOrder", "--message-body", "test")]
//     [TestCase("Error: Option 'destination-endpoint' is defined but no value has been provided.", "command", "send", "--content-type", "application/json", "--enclosed-message-type", "MessageContracts.Commands.CreateOrder", "--message-body", "test", "--destination-endpoint")]
//     public async Task ShouldValidateRequiredOptions(string expectedSubstring, params string[] args)
//     {
//         var result = await _sut.RunAsync([args]);
//
//         Assert.That(result.ExitCode, Is.Not.EqualTo(0));
//         Assert.That(result.Output, Does.Contain(expectedSubstring), $"Expected output to contain: {expectedSubstring}");
//     }
// }