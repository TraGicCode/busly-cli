# busly timeout send

Send a timeout message to an endpoint.

## Usage

```
busly timeout send
```

## Options

| Option                          | Description                                                                                                       |
| ------------------------------- | ----------------------------------------------------------------------------------------------------------------- |
| `-c`, `--content-type`          | The fully qualified .NET type name of the enclosed message (ex: Ordering.Commands.CreateOrder )                   |
| `-e`, `--enclosed-message-type` | The type of serialization used for the message                                                                    |
| `-m`, `--message-body`          | The content of the message body                                                                                   |
| `-d`, `--destination-endpoint`  | The destination endpoint to send a message to                                                                     |
| `--do-not-deliver-before`       | Allows specifying a date before which the delivery should not occur, using ISO-8601 format (YYYY-MM-DDTHH:mm:ssZ) |
| `--delay-delivery-with`         | Specifies the delay before the timeout is delivered, using a TimeSpan format                                      |

## Examples

```

```