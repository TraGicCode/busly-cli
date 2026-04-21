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
| `-m`, `--message-body`          | The content of the message body. Accepts a raw JSON string or a path to a file using curl-style `@` syntax (e.g. `@payload.json`). |
| `-d`, `--destination-endpoint`  | The destination endpoint to send a message to                                                                     |
| `--do-not-deliver-before`       | Allows specifying a date before which the delivery should not occur, using ISO-8601 format (YYYY-MM-DDTHH:mm:ssZ) |
| `--delay-delivery-with`         | Specifies the delay before the timeout is delivered, using a TimeSpan format                                      |

## Transport Support

Not all transports support sending timeouts. Transports that delegate delayed delivery to the broker work fine. Transports that rely on an in-process poller to forward deferred messages are not compatible with the CLI's fire-and-forget execution model — the process exits before the poller has a chance to dispatch the message.

| Transport              | Supported | Reason                                                              |
| ---------------------- | --------- | ------------------------------------------------------------------- |
| Learning               | ✅         | Broker-side delayed delivery                                        |
| RabbitMQ               | ✅         | Broker-side delayed delivery via TTL                                |
| Azure Service Bus      | ✅         | Broker-side scheduled messages                                      |
| Amazon SQS             | ✅         | Native SQS message delay                                            |
| Azure Storage Queues   | ❌         | Requires an in-process poller; exits before messages become due     |
| PostgreSQL             | ❌         | Requires an in-process poller; send-only endpoints not supported    |
| SQL Server             | ❌         | Requires an in-process poller; send-only endpoints not supported    |

## Examples

```

```