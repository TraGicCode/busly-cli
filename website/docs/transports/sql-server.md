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
