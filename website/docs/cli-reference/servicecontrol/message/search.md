# busly servicecontrol message search

Search for successful messages processed by endpoints.

## Usage

```
busly servicecontrol message search [keyword]
```

## Aliases

- `s`

## Arguments

| Argument  | Description                                     |
| --------- | ----------------------------------------------- |
| `keyword` | Optional keyword to filter messages by.         |

## Options

| Option                       | Description                                                                                                      |
| ---------------------------- | ---------------------------------------------------------------------------------------------------------------- |
| `--endpoint <endpoint>`      | The endpoint to search messages for.                                                                             |
| `--page-size <page-size>`    | The number of results to return per page. (Default: `50`)                                                        |
| `--from <from>`              | Start of the date range to search, in ISO-8601 format (`YYYY-MM-DDTHH:mm:ssZ`). Must be used with `--to`.       |
| `--to <to>`                  | End of the date range to search, in ISO-8601 format (`YYYY-MM-DDTHH:mm:ssZ`). Must be used with `--from`.       |
| `--sort <sort>`              | Field to sort results by. Allowed values: `TimeSent`, `ProcessingTime`, `CriticalTime`, `DeliveryTime`. (Default: `TimeSent`) |
| `--sort-direction <dir>`     | Sort direction. Allowed values: `Asc`, `Desc`. (Default: `Desc`)                                                |

## Examples

```bash
# Search all messages
busly servicecontrol message search

# Search by keyword
busly servicecontrol message search "OrderPlaced"

# Search messages for a specific endpoint
busly servicecontrol message search --endpoint "Sales"

# Search messages within a date range
busly servicecontrol message search --from 2026-05-01T00:00:00Z --to 2026-05-11T23:59:59Z

# Search with custom sort and page size
busly servicecontrol message search --sort ProcessingTime --sort-direction Asc --page-size 100
```
