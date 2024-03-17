using Ekzakt.EmailSender.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Constants;
using System.Text.Json.Serialization;

namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailTemplate : Email
{
    public string RecipientType { get; set; } = string.Empty;

    public List<EmailAddress> Tos { get; set; } = new();

    public List<EmailAddress>? Ccs { get; set; } = new();

    public List<EmailAddress>? Bccs { get; set; } = new();

    public string Subject { get; set; } = string.Empty;

    public TemplateBody Body { get; set; } = new();

    public List<EmailAddress> Tos { get; set; } = new();

    public List<EmailAddress>? Ccs { get; set; } = new();

    public List<EmailAddress>? Bccs { get; set; } = new();

    public string Subject { get; set; } = string.Empty;

    public TemplateBody Body { get; set; } = new();

    public List<EmailAddress> Tos { get; set; } = new();

    public List<EmailAddress>? Ccs { get; set; } = new();

    public List<EmailAddress>? Bccs { get; set; } = new();

    public string Subject { get; set; } = string.Empty;

    public TemplateBody Body { get; set; } = new();


    [JsonIgnore]
    public bool IsAdmin => RecipientType == RecipientTypes.USER;

    [JsonIgnore]
    public bool IsUser => RecipientType == RecipientTypes.USER;
}
