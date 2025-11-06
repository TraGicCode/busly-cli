using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NServiceBusCLI.Commands;

// TODO: Required options will be implemented soon according to https://github.com/spectreconsole/spectre.console/discussions/538.  Until this is implemented we must do the following
//  public override ValidationResult Validate()
// {
//     if (string.IsNullOrWhiteSpace(Foo))
//     {
//         return ValidationResult.Error("Foo is required");
//     }
//
//     return base.Validate();
// }
public abstract class CommonMessageSettings : GlobalCommandSettings
{
    [CommandOption("-c|--content-type <content-type>")]
    [Description("The type of serialization used for the message")]
    public required string ContentType { get; set; }

    [CommandOption("-e|--enclosed-message-type <enclosed-message-type>")]
    [Description("The fully qualified .NET type name of the enclosed message")]
    public required string EnclosedMessageType { get; set; }

    [CommandOption("-m|--message-body <message-body>")]
    [Description("The content of the message body")]
    public required string MessageBody { get; set; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(ContentType)) return ValidationResult.Error("must specify a 'content-type'.");
        if (string.IsNullOrWhiteSpace(EnclosedMessageType))
            return ValidationResult.Error("must specify an 'enclosed-message-type'.");
        if (string.IsNullOrWhiteSpace(MessageBody)) return ValidationResult.Error("must specify a 'message-body'.");
        return base.Validate();
    }

    // public override ValidationResult Validate()
    // {
    // if (!string.IsNullOrWhiteSpace(ConnectionString)
    //     && (!string.IsNullOrWhiteSpace(Server)
    //         || !string.IsNullOrWhiteSpace(Database)))
    // {
    //     return ValidationResult.Error(
    //         "You cannot specify a <Server> or <Database> when passing a <ConnectionString>");
    // }
    //
    // if (string.IsNullOrEmpty(ConnectionString) && !string.IsNullOrWhiteSpace(Server) &&
    //     string.IsNullOrWhiteSpace(Database))
    //     return ValidationResult.Error("You must specify a <database> when using the <server> parameter");
    //
    // if (string.IsNullOrEmpty(ConnectionString) && !string.IsNullOrWhiteSpace(Database) &&
    //     string.IsNullOrWhiteSpace(Server))
    //     return ValidationResult.Error("You must specify a <server> when using the <database> parameter");
    //
    // if (!string.IsNullOrWhiteSpace(UserID) && string.IsNullOrWhiteSpace(Password))
    // {
    //     return ValidationResult.Error("You must specify a <Password> when passing a <UserID>");
    // }
    //
    // if (!string.IsNullOrWhiteSpace(Password) && string.IsNullOrWhiteSpace(UserID))
    // {
    //     return ValidationResult.Error("You must specify a <UserID> when passing a <Password>");
    // }
    //
    //     return base.Validate();
    // }
}