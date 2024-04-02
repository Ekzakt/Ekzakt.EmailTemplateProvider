using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.Utilities;

namespace Ekzakt.EmailTemplateProvider.Core.Extensions;

public static class EmailTemplateInfoExtensions
{
    public static EmailTemplateInfo ApplyReplacements(this EmailTemplateInfo templates, StringReplacer replacer)
    {
        if (!templates.IsValid)
        {
            return templates;
        }

        foreach (var emailSetting in templates.EmailInfos!)
        {
            var email = emailSetting.Email;

            if (email != null)
            { 
                email.Subject = replacer.Replace(email!.Subject ?? string.Empty);
                email.Body.Html = replacer.Replace(email!.Body.Html);
                email.Body.Text = replacer.Replace(email!.Body.Text ?? string.Empty);
            }
        }

        return templates;
    }
}
