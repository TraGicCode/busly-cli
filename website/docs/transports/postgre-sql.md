# Postgre Sql

The **Postgre Sql Transport** is used to communicate to Postgre Sql. It is suitable for development, testing, and production environments.

## Configuration

To use the Postgre Sql Transport, define it under `transports` and reference it as `current-transport`.

### Example

```yaml
current-transport: local-postgre-sql

transports:
  - name: local-postgre-sql
    postgre-sql-transport-config:
      connection-string: Data Source=(local);Initial Catalog=Ordering;Integrated Security=SSPI;Application Name=Busly-CLI;TrustServerCertificate=true
```

---

## `postgre-sql-transport-config` Fields

| Field               | Required | Type   | Default | Description                         |
| ------------------- | -------- | ------ | ------- | ----------------------------------- |
| `connection-string` | **Yes**  | string | —       | Full Postgre Sql Connection string. |

---

## Field Details

### `connection-string` (required)

Postgre Sql connection string used to connect to the database.

Examples:

```yaml
connection-string: Data Source=(local);Initial Catalog=Ordering;Integrated Security=SSPI;Application Name=Busly-CLI;TrustServerCertificate=true
```

## Limitations

### Timeout Send Not Supported

The `timeout send` command is not supported with the PostgreSQL transport. PostgreSQL implements delayed delivery using a database table polled by an in-process background worker. Because busly exits immediately after dispatching a message, the poller is stopped before it can forward the deferred message to the destination queue. Additionally, the transport explicitly blocks delayed delivery from send-only endpoints.
