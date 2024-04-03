using Ekzakt.EmailSender.Core.Models;

namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailInfo : ICloneable
{
    public string RecipientType { get; set; } = string.Empty;

    public Email? Email { get; set; }

    public object Clone()
    {
        EmailInfo cloned = (EmailInfo)MemberwiseClone();
        cloned.RecipientType = RecipientType;
        cloned.Email = Email;

        return cloned;
    }
}
