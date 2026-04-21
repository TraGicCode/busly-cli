# busly event publish

Publish an event to subscribing endpoints.

## Usage

```
busly event publish
```

## Options

| Option                          | Description                                                                                   |
| ------------------------------- | --------------------------------------------------------------------------------------------- |
| `-c`, `--content-type`          | The type of serialization used for the message                                                |
| `-e`, `--enclosed-message-type` | The fully qualified .NET type name of the enclosed message (ex: Ordering.Events.OrderPlaced ) |
| `-m`, `--message-body`          | The content of the message body. Accepts a raw JSON string or a path to a file using curl-style `@` syntax (e.g. `@payload.json`). |

## Examples

```bash
# Publish an event with inline JSON
busly event publish \
  --content-type "text/json" \
  --enclosed-message-type "Messages.Events.OrderPlaced" \
  --message-body '{"OrderNumber":"3f2d6c8a-b7a2-4c3f-9c3e-12ab45ef6789"}'
```

```bash
# Publish an event using a JSON file
busly event publish \
  --content-type "text/json" \
  --enclosed-message-type "Messages.Events.OrderPlaced" \
  --message-body @payload.json
```
