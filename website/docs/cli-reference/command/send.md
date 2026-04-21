# busly command send

Send a one-way command to an endpoint.

## Usage

```
busly send command
```

## Options

| Option                          | Description                                                                                     |
| ------------------------------- | ----------------------------------------------------------------------------------------------- |
| `-c`, `--content-type`          | The type of serialization used for the message                                                  |
| `-e`, `--enclosed-message-type` | The fully qualified .NET type name of the enclosed message (ex: Ordering.Commands.CreateOrder ) |
| `-m`, `--message-body`          | The content of the message body. Accepts a raw JSON string or a path to a file using curl-style `@` syntax (e.g. `@payload.json`). |
| `-d`, `--destination-endpoint`  | The destination endpoint to send the command to                                                 |

## Examples

```bash
# Send a command with inline JSON
busly command send \
  --content-type "text/json" \
  --enclosed-message-type "Messages.Commands.CreateOrder" \
  --destination-endpoint "Ordering" \
  --message-body '{"OrderNumber":"3f2d6c8a-b7a2-4c3f-9c3e-12ab45ef6789"}'
```

```bash
# Send a command using a JSON file
busly command send \
  --content-type "text/json" \
  --enclosed-message-type "Messages.Commands.CreateOrder" \
  --destination-endpoint "Ordering" \
  --message-body @payload.json
```
