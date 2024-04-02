using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;

namespace Ekzakt.EmailTemplateProvider.Core.Caching;

public interface IEmailTemplateCache
{
    Task<(bool, EmailTemplateInfo?)> TryGetTemplate(EmailTemplateRequest request, Func<EmailTemplateRequest, Task<EmailTemplateInfo?>> cacheKeyNotFound);

    void SetTemplate(string cacheKeyName, EmailTemplateInfo emailTemplateInfo);
}