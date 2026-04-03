using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Sonara.Infrastructure.Configuration;

namespace Sonara.Infrastructure.Services;

public sealed class SupabaseStorageClient
{
    private readonly HttpClient _http;
    private readonly SupabaseStorageOptions _options;

    public SupabaseStorageClient(HttpClient http, IOptions<SupabaseStorageOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    private void EnsureConfigured()
    {
        if (!_options.IsConfigured)
            throw new InvalidOperationException("Supabase storage is not configured.");
    }

    private string StorageRoot => $"{_options.Url!.TrimEnd('/')}/storage/v1";

    private static void AddSupabaseAuth(HttpRequestMessage request, SupabaseStorageOptions options)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ServiceRoleKey);
        request.Headers.Remove("apikey");
        request.Headers.TryAddWithoutValidation("apikey", options.ServiceRoleKey!);
    }

    public async Task UploadObjectAsync(
        string objectPath,
        Stream content,
        string? contentType,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        var path = objectPath.TrimStart('/');
        var url = $"{StorageRoot}/object/{_options.Bucket}/{path}";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        AddSupabaseAuth(request, _options);
        request.Headers.TryAddWithoutValidation("x-upsert", "true");
        request.Content = new StreamContent(content);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);

        var response = await _http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Supabase upload failed ({(int)response.StatusCode}): {body}");
        }
    }

    public async Task<string> CreateSignedUrlAsync(
        string objectPath,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();
        var path = objectPath.TrimStart('/');
        var url = $"{StorageRoot}/object/sign/{_options.Bucket}/{path}";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        AddSupabaseAuth(request, _options);
        request.Content = JsonContent.Create(new { expiresIn = _options.SignedUrlExpirySeconds });

        var response = await _http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Supabase sign failed ({(int)response.StatusCode}): {body}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var payload = await System.Text.Json.JsonSerializer.DeserializeAsync(
            stream,
            SignedUrlPayloadJsonContext.Default.SignedUrlPayload,
            cancellationToken);
        var relative = payload?.SignedURL ?? payload?.SignedUrl;
        if (string.IsNullOrEmpty(relative))
            throw new InvalidOperationException("Supabase sign response missing signedURL.");

        if (relative.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || relative.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return relative;

        var baseUrl = _options.Url!.TrimEnd('/');
        return relative.StartsWith('/')
            ? $"{baseUrl}{relative}"
            : $"{baseUrl}/{relative}";
    }

    public async Task DeleteObjectAsync(string objectPath, CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
            return;

        var path = objectPath.TrimStart('/');
        var url = $"{StorageRoot}/object/{_options.Bucket}/{path}";
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        AddSupabaseAuth(request, _options);
        using var response = await _http.SendAsync(request, cancellationToken);
        _ = response;
    }
}

internal sealed class SignedUrlPayload
{
    [JsonPropertyName("signedURL")]
    public string? SignedURL { get; set; }

    [JsonPropertyName("signedUrl")]
    public string? SignedUrl { get; set; }
}

[JsonSerializable(typeof(SignedUrlPayload))]
internal partial class SignedUrlPayloadJsonContext : JsonSerializerContext;
