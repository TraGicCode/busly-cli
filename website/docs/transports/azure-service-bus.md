# Azure Service Bus

The **Azure Service Bus Transport** is used to communicate to Azure Service Bus.
It is suitable for development, testing, and production environments.

## Configuration

To use the Azure Service Bus Transport, define it under `transports` and reference it as `current-transport`.

### Example

```yaml
current-transport: local-azure-service-bus

transports:
  - name: local-azure-service-bus
    azure-service-bus-transport-config:
      connection-string: Endpoint=amqp://127.0.0.1:32799/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true
```

---

## `azure-service-bus-transport-config` Fields

| Field               | Required | Type   | Default | Description                                                       |
|---------------------|----------|--------|---------|-------------------------------------------------------------------|
| `connection-string` | **Yes**  | string | —       | Full AMQP connection string used to connect to Azure Service Bus. |

---

## Field Details

### `connection-string` (required)

A standard AMQP URI used to connect to Azure Service Bus.

Examples:

```yaml
amqp-connection-string: Endpoint=sb://[NAMESPACE].servicebus.windows.net/;SharedAccessKeyName=[KEYNAME];SharedAccessKey=[KEY]
```

```yaml
amqp-connection-string: Endpoint=amqp://127.0.0.1:32799/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true
```
