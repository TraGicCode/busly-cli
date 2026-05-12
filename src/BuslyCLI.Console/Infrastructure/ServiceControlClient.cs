using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BuslyCLI.Infrastructure.ServiceControl;

namespace BuslyCLI.Infrastructure;

public class ServiceControlClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public async Task<IReadOnlyList<ServiceControlEndpoint>> GetEndpointsAsync(string baseUrl, CancellationToken cancellationToken = default)
    {
        var url = $"{baseUrl.TrimEnd('/')}/endpoints";
        return await httpClient.GetFromJsonAsync<List<ServiceControlEndpoint>>(url, JsonOptions, cancellationToken) ?? [];
    }

    public async Task<bool> DeleteEndpointAsync(string baseUrl, Guid id, CancellationToken cancellationToken = default)
    {
        var url = $"{baseUrl.TrimEnd('/')}/endpoints/{id}";
        var response = await httpClient.DeleteAsync(url, cancellationToken);

        if (response.IsSuccessStatusCode)
            return true;

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        response.EnsureSuccessStatusCode();
        return false;
    }

    public async Task<IReadOnlyList<ServiceControlEventLogItem>> GetEventLogItemsAsync(string baseUrl, int page = 1, int perPage = 20, CancellationToken cancellationToken = default)
    {
        var url = $"{baseUrl.TrimEnd('/')}/eventlogitems?page={page}&per_page={perPage}";
        return await httpClient.GetFromJsonAsync<List<ServiceControlEventLogItem>>(url, JsonOptions, cancellationToken) ?? [];
    }

    public async Task<ServiceControlLicense> GetLicenseAsync(string baseUrl, CancellationToken cancellationToken = default)
    {
        var url = $"{baseUrl.TrimEnd('/')}/license";
        return await httpClient.GetFromJsonAsync<ServiceControlLicense>(url, JsonOptions, cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceControlCustomCheck>> GetCustomChecksAsync(string baseUrl, string status = null, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string> { $"page={page}", $"pageSize={pageSize}" };

        if (!string.IsNullOrEmpty(status))
            queryParams.Add($"status={Uri.EscapeDataString(status)}");

        var url = $"{baseUrl.TrimEnd('/')}/customchecks?{string.Join("&", queryParams)}";
        return await httpClient.GetFromJsonAsync<List<ServiceControlCustomCheck>>(url, JsonOptions, cancellationToken) ?? [];
    }

    public async Task<IReadOnlyList<ServiceControlMessage>> SearchMessagesAsync(
        string baseUrl,
        string q = null,
        string endpointName = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        uint pageSize = 50,
        string sort = "time_sent",
        string direction = "desc",
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(endpointName))
            queryParams.Add($"endpoint_name={Uri.EscapeDataString(endpointName)}");

        if (from.HasValue)
            queryParams.Add($"from={Uri.EscapeDataString(from.Value.UtcDateTime.ToString("o"))}");

        if (to.HasValue)
            queryParams.Add($"to={Uri.EscapeDataString(to.Value.UtcDateTime.ToString("o"))}");

        if (!string.IsNullOrEmpty(q))
            queryParams.Add($"q={Uri.EscapeDataString(q)}");

        queryParams.Add($"page_size={pageSize}");
        queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
        queryParams.Add($"direction={Uri.EscapeDataString(direction)}");

        var queryString = string.Join("&", queryParams);
        var url = $"{baseUrl.TrimEnd('/')}/messages2/?{queryString}";

        return await httpClient.GetFromJsonAsync<List<ServiceControlMessage>>(url, JsonOptions, cancellationToken) ?? [];
    }
}