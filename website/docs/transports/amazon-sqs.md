# Amazon SQS

The **Amazon SQS Transport** is used to communicate to Amazon SQS. It is suitable for development, testing, and production environments.

## Configuration

To use the Amazon SQS Transport, define it under `transports` and reference it as `current-transport`.

### Example

```yaml
current-transport: local-stack-amazon-sqs

transports:
  - name: local-stack-amazon-sqs
    amazonsqs-transport-config:
      region-name: us-east-1
      access-key: test
      secret-key: test
      service-url: http://127.0.0.1:32813/ # (optional) Only used when connecting to local-stack
```

:::info

The Amazon SQS transport implementation currently works only with **AWS access key and secret key authentication**. Pull requests that add support for additional authentication methods are welcome and greatly appreciated!

:::

---

## `amazonsqs-transport-config` Fields

| Field         | Required | Type   | Default | Description                                                                                                                                                                       |
| ------------- | -------- | ------ | ------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `region-name` | **Yes**  | string | —       | The AWS region. All Region codes can be found [here](https://docs.aws.amazon.com/global-infrastructure/latest/regions/aws-regions.html) (EX: us-east-1, us-east2, us-west1..etc). |
| `access-key`  | **Yes**  | string | —       | The AWS Access Key.                                                                                                                                                               |
| `secret-key`  | **Yes**  | string | —       | The AWS Secret Key.                                                                                                                                                               |
| `service-url` | No       | string | —       | The service URL used to connect to LocalStack for local development.                                                                                                              |

---

## Field Details

### `region-name` (required)

The AWS Region SQS is hosted in.

Examples:

```yaml
region-name: us-east-1
```

---

### `access-key` (required)

The AWS Access Key.

Examples:

```yaml
access-key: test
```

---

### `secret-key` (required)

The AWS Secret Key.

Examples:

```yaml
secret-key: test
```

---

### `service-url` (optional)

The service URL used to connect to LocalStack for local development.

Examples:

```yaml
service-url: http://127.0.0.1:32813/
```
