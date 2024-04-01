using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.Utilities;

namespace Ekzakt.EmailTemplateProvider.Core.Extensions;

public static class EmailTemplateSettingsExtensions
{
    public static EmailTemplateSettings ApplyReplacements(this EmailTemplateSettings templates, StringReplacer replacer)
    {
        if (!templates.IsValid)
        {
            return templates;
        }

        foreach (var emailSetting in templates.EmailSettings!)
        {
            var email = emailSetting.Email;

            replacer.Replace(email!.Subject ?? string.Empty);
            replacer.Replace(email!.Body.Html);
            replacer.Replace(email!.Body.Text ?? string.Empty);
        }

        return templates;
    }
}
