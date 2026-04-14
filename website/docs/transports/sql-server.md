# Sql Server

The **Sql Server Transport** is used to communicate to Microsoft Sql Server. It is suitable for development, testing, and production environments.

## Configuration

To use the Sql Server Transport, define it under `transports` and reference it as `current-transport`.

### Example

```yaml
current-transport: local-sql-server

transports:
  - name: local-sql-server
    sql-server-transport-config:
      connection-string: Data Source=(local);Initial Catalog=Ordering;Integrated Security=SSPI;Application Name=Busly-CLI;TrustServerCertificate=true
```

---

## `sql-server-transport-config` Fields

| Field               | Required | Type   | Default | Description                        |
| ------------------- | -------- | ------ | ------- | ---------------------------------- |
| `connection-string` | **Yes**  | string | —       | Full Sql Server Connection string. |

---

## Field Details

### `connection-string` (required)

Sql Server connection string used to connect to Microsoft SQL Server.

Examples:

```yaml
connection-string: Data Source=(local);Initial Catalog=Ordering;Integrated Security=SSPI;Application Name=Busly-CLI;TrustServerCertificate=true
```

## Limitations

### Timeout Send Not Supported

The `timeout send` command is not supported with the SQL Server transport. SQL Server implements delayed delivery using a database table polled by an in-process background worker. Because busly exits immediately after dispatching a message, the poller is stopped before it can forward the deferred message to the destination queue. Additionally, the transport explicitly blocks delayed delivery from send-only endpoints.
