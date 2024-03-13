namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailTemplate
{
    public string RecipientType { get; set; } = string.Empty;

    public EmailAddress? From { get; set; }

    public List<EmailAddress> Tos { get; set; } = new();

    public List<EmailAddress>? Ccs { get; set; } = new();

    public List<EmailAddress>? Bccs { get; set; } = new();

    public string? Subject { get; set; }

    public TemplateBody Body { get; set; } = new();
}
