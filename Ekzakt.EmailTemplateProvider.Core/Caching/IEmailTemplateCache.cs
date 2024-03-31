using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;

namespace Ekzakt.EmailTemplateProvider.Core.Caching;

public interface IEmailTemplateCache
{
    Task<(bool, EmailTemplateSettings?)> TryGetTemplate(EmailTemplateRequest request, Func<EmailTemplateRequest, Task<EmailTemplateSettings?>> cacheKeyNotFound);

    void SetTemplate(string cacheKeyName, EmailTemplateSettings emailTemplateSettings);
}