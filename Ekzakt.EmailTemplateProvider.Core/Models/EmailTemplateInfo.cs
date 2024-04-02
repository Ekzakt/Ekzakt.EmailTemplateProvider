namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailTemplateInfo
{
    public string? TenantId { get; set; }

    public string CultureName { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public List<EmailInfo>? EmailInfos { get; set; }

    public bool IsValid =>
        !string.IsNullOrWhiteSpace(CultureName) ||
        !string.IsNullOrWhiteSpace(TemplateName) ||
        EmailInfos?.Count > 0;
}
