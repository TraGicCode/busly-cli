# busly command send

Send a one-way command to an endpoint.

## Usage

```
busly send command
```

## Options

| Option                          | Description                                                                                     |
| ------------------------------- | ----------------------------------------------------------------------------------------------- |
| `-c`, `--content-type`          | The fully qualified .NET type name of the enclosed message (ex: Ordering.Commands.CreateOrder ) |
| `-e`, `--enclosed-message-type` | The type of serialization used for the message                                                  |
| `-m`, `--message-body`          | The content of the message body. Accepts a raw JSON string or a path to a file using curl-style `@` syntax (e.g. `@payload.json`). |

## Examples

```

```
