using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.Utilities;

namespace Ekzakt.EmailTemplateProvider.Core.Extensions;

public static class EmailTemplateInfoExtensions
{
    public static EmailTemplateInfo ApplyReplacements(this EmailTemplateInfo templateInfo, StringReplacer replacer)
    {
        if (!templateInfo.IsValid)
        {
            return templateInfo;
        }

        foreach (var emailInfo in templateInfo.EmailInfos!)
        {
            var email = emailInfo.Email;

            if (email != null)
            { 
                email.Subject = replacer.Replace(email!.Subject ?? string.Empty);
                email.Body.Html = replacer.Replace(email!.Body.Html);
                email.Body.Text = replacer.Replace(email!.Body.Text ?? string.Empty);
            }

            emailInfo.Email = email;
        }

        return templateInfo;
    }
}
