# Amazon SQS

The **Amazon SQS Transport** is used to communicate to Amazon SQS.

## Configuration

To use the Amazon SQS Transport, define it under `transports` and reference it as `current-transport`.

### Example

```yaml
current-transport: local-stack-amazon-sqs

transports:
  - name: local-stack-amazon-sqs
    amazonsqs-transport-config:
      service-url: http://127.0.0.1:32813/
      region-name: us-east-1
```

:::info

The Amazon SQS transport implementation currently works only with the LocalStack emulator.
Pull requests to improve functionality or add support for live AWS SQS are welcome and much appreciated!

:::

---

## `amazonsqs-transport-config` Fields

| Field         | Required | Type   | Default | Description                     |
| ------------- | -------- | ------ | ------- | ------------------------------- |
| `service-url` | **Yes**  | string | —       | The Service Url for Amazon SQS. |
| `region-name` | **Yes**  | string | —       | The region (EX: us-east-1).     |

---

## Field Details

### `service-url` (required)

The Service Url for Amazon SQS.

Examples:

```yaml
service-url: http://127.0.0.1:32813/
```

### `region-name` (optional)

Allows Busly to interact with the RabbitMQ Management API for monitoring or queue management.

Examples:

```yaml
region-name: us-east-1
```
