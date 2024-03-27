using Ekzakt.EmailSender.Core.Models;

namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailSettings
{
    public string RecipientType { get; set; } = string.Empty;

    public Email? Email { get; set; }
}
