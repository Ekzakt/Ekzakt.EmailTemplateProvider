using System.Text.Json;

namespace Ekzakt.EmailTemplateProvider.Core.Requests;

public class EmailTemplateRequest
{
    private string? _tenantId;
    private string _cultureName = string.Empty;
    private string _templateName = string.Empty;


    public string? TenantId
    {
        get { return _tenantId; }
        set { _tenantId = value?.ToLower(); }
    }


    public string CultureName
    {
        get { return _cultureName; }
        set { _cultureName = value.ToLower(); }
    }


    public string TemplateName
    {
        get { return _templateName; }
        set { _templateName = value.ToLower(); }
    }


    public string CacheKeyName => $"{TenantId}{(TenantId != null ? "." : string.Empty)}{CultureName}.{TemplateName}";

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
