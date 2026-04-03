namespace Sonara.Infrastructure.Configuration;

public sealed class SupabaseStorageOptions
{
    public const string SectionName = "SupabaseStorage";
    public string? Url { get; set; }
    public string? ServiceRoleKey { get; set; }
    public string Bucket { get; set; } = "sonara";
    public int SignedUrlExpirySeconds { get; set; } = 3600;
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(ServiceRoleKey);
}