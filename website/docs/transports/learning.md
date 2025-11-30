# Learning Transport

The **Learning Transport** is a simple file-based transport used mainly for local development and tutorials.

## Configuration

To use the Learning Transport, define it under `transports` and reference it as `current-transport`.

### Example

```yaml
current-transport: local-learning

transports:
    - name: local-learning
      learning-transport-config:
          storage-directory: C:\Source\tutorials-quickstart\.learningtransport
          restrict-payload-size: true
```

---

## `learning-transport-config` Fields

| Field                   | Required | Type    | Default | Description                                                                                           |
| ----------------------- | -------- | ------- | ------- | ----------------------------------------------------------------------------------------------------- |
| `storage-directory`     | **Yes**  | string  | —       | Absolute path where Learning Transport stores message files. Busly will not start without this value. |
| `restrict-payload-size` | No       | boolean | `true`  | Enforces the NServiceBus payload size limit. Set to `false` if you need to send larger payloads.      |

---

## Field Details

### `storage-directory` (required)

This must be an **absolute file path**.
The Learning Transport stores outgoing and incoming messages in a folder structure at this location.
If this value is missing, Busly reports an error and exits.

Example:

```yaml
storage-directory: C:\MyProject\.learningtransport
```

---

### `restrict-payload-size` (optional, default: `true`)

When enabled (the default), Busly enforces the same message size limits as NServiceBus.

Set to `false` if using large JSON payloads or test messages.

Example:

```yaml
restrict-payload-size: false
```
