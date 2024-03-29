namespace Ekzakt.EmailTemplateProvider.Core.Caching;

public class CacheKeyNotFoundEventArgs : EventArgs
{
    public string? TenantId { get; set; }

    public string CultureName { get; set; } = string.Empty;

    public string TemplateName { get; set;} = string.Empty;
}
