using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;

namespace Ekzakt.EmailTemplateProvider.Core.Caching;

public interface IEmailTemplatesCache
{
    delegate Task AsyncEventHandler<TEventArgs>(TEventArgs e);

    event AsyncEventHandler<CacheKeyNotFoundEventArgs> CacheKeyNotFound;

    Task<EmailTemplateSettings?> GetTemplateAsync(EmailTemplatesRequest request, string? fallbackCultureName = null);

    void SetTemplate(EmailTemplateSettings emailTemplateSettings);
}