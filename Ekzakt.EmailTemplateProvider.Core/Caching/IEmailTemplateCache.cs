using Ekzakt.EmailTemplateProvider.Core.Models;

namespace Ekzakt.EmailTemplateProvider.Core.Caching;

public interface IEmailTemplateCache
{
    EmailTemplateSettings? GetTemplate(string cultureName, string templateName, string falbackCultureName);

    void SetTemplate(EmailTemplateSettings emailTemplateSettings);
}