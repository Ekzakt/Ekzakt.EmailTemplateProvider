using Ekzakt.EmailTemplateProvider.Core.Constants;
using System.Text.Json.Serialization;

namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailTemplate
{
    public string RecipientType { get; set; } = string.Empty;

    public Email? 


    [JsonIgnore]
    public bool IsAdmin => RecipientType == RecipientTypes.USER;

    [JsonIgnore]
    public bool IsUser => RecipientType == RecipientTypes.USER;
}
