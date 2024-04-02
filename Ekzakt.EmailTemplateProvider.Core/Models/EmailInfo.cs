using Ekzakt.EmailSender.Core.Models;

namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailInfo
{
    public string RecipientType { get; set; } = string.Empty;

    public Email? Email { get; set; }
}
