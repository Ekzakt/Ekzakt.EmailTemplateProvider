namespace Ekzakt.EmailTemplateProvider.Core.Requests;

public class EmailTemplatesRequest
{
    public string? TenantId { get; set; }

    public string CultureName { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public string CacheKey => $"{TenantId}{(TenantId != null ? "." : string.Empty)}{CultureName}.{TemplateName}".ToLower();

}
