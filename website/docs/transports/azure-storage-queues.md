# Azure Storage Queues

The **Azure Storage Queues Transport** is used to communicate to Azure Storage Queues. It is suitable for development, testing, and production environments.

## Configuration

To use the Azure Storage Queues Transport, define it under `transports` and reference it as `current-transport`.

### Example

```yaml
current-transport: local-azure-service-bus

transports:
  - name: local-azure-service-bus
    azure-storage-queues-transport-config:
      connection-string: Endpoint=amqp://127.0.0.1:32799/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true
```

---

## `azure-storage-queues-transport-config` Fields

| Field               | Required | Type   | Default | Description                                |
| ------------------- | -------- | ------ | ------- | ------------------------------------------ |
| `connection-string` | **Yes**  | string | —       | Connection string to Azure Storage Queues. |

---

## Field Details

### `connection-string` (required)

A standard conection string for Azure Storage Queues.

Examples:

```yaml
connection-string: UseDevelopmentStorage=true
```
