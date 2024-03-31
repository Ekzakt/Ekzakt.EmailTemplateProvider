using System.Text.Json;

namespace Ekzakt.EmailTemplateProvider.Core.Requests;

public class EmailTemplateRequest
{
    public string? TenantId { get; set; }

    public string CultureName { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public string CacheKeyName => $"{TenantId}{(TenantId != null ? "." : string.Empty)}{CultureName}.{TemplateName}".ToLower();

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
