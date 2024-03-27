namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailTemplateSettings
{
    public string CultureName { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public List<EmailSettings>? EmailSettings { get; set; }

    public bool IsValid =>
        !string.IsNullOrWhiteSpace(CultureName) ||
        !string.IsNullOrWhiteSpace(TemplateName) ||
        EmailSettings?.Count > 0;
}
