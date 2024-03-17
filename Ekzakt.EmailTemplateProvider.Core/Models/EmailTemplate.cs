using Ekzakt.EmailSender.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Constants;
using System.Text.Json.Serialization;

namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailTemplate : Email
{
    public string RecipientType { get; set; } = string.Empty;


    [JsonIgnore]
    public bool IsAdmin => RecipientType == RecipientTypes.ADMIN;

    [JsonIgnore]
    public bool IsUser => RecipientType == RecipientTypes.USER;
}
