# RabbitMQ

The **RabbitMQ Transport** is used to communicate to RabbitMQ.
It is suitable for development, testing, and production environments.

## Configuration

To use the RabbitMQ Transport, define it under `transports` and reference it as `current-transport`.

### Example

```yaml
current-transport: local-rabbitmq

transports:
  - name: local-rabbitmq
    rabbitmq-transport-config:
      amqp-connection-string: amqp://localhost
```

---

## `rabbitmq-transport-config` Fields

| Field                    | Required | Type   | Default      | Description                                                       |
| ------------------------ | -------- | ------ | ------------ | ----------------------------------------------------------------- |
| `amqp-connection-string` | **Yes**  | string | —            | Full AMQP connection string used to connect to RabbitMQ.          |
| `routing-toplogy`        | No       | string | conventional | Routing toplogy to be used (Conventional or Direct).              |
| `queue-type`             | No       | string | quorum       | Type of queues RabbitMQ is using (Quorum or Classic).             |
| `management-api`         | No       | object | —            | Optional configuration to connect to the RabbitMQ Management API. |

---

## `management-api` Fields

| Field      | Required | Type   | Default | Description                                                                                                   |
| ---------- | -------- | ------ | ------- | ------------------------------------------------------------------------------------------------------------- |
| `url`      | No       | string | —       | Base URL for the RabbitMQ Management API. Example: http://localhost:15672.                                    |
| `username` | No       | string | —       | Username for authenticating with the Management API. If username is provided, password must also be provided. |
| `password` | No       | string | —       | Password for authenticating with the Management API. If password is provided, username must also be provided. |

---

## Field Details

### `amqp-connection-string` (required)

A standard AMQP URI used to connect to RabbitMQ.

Examples:

```yaml
amqp-connection-string: amqp://guest:guest@localhost:5672/
```

```yaml
amqp-connection-string: amqps://user:pass@rabbitmq.example.com:5671/my-vhost
```

---

### `routing-topology` (optional)

The Routing topology to be used for the transport

Examples:

```yaml
routing-toplogy: conventional
```

```yaml
routing-toplogy: direct
```

---

### `queue-type` (optional)

The type of queues being used.

Examples:

```yaml
queue-type: quorum
```

```yaml
queue-type: classic
```

---

### `management-api` (optional)

Allows Busly to interact with the RabbitMQ Management API for monitoring or queue management.

Examples:

```yaml
management-api:
  url: http://localhost:15672
```

```yaml
management-api:
  username: admin
  password: hello123!
```

```yaml
management-api:
  url: http://localhost:15672
  username: admin
  password: hello123!
```
