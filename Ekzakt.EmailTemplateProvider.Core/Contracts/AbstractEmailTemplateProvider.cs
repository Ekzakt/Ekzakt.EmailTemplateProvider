using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.EmailTemplateProvider.Core.Responses;

namespace Ekzakt.EmailTemplateProvider.Core.Contracts;

public abstract class AbstractEmailTemplateProvider : IEkzaktEmailTemplateProvider
{
    public async Task<EmailTemplatesResponse> GetEmailTemplatesAsync(EmailTemplatesRequest request, CancellationToken cancellationToken = default)
    {
        var templates = TryGetFromCache(request.TemplateName, request.CultureName);

        if (templates is not null)
        {
            return new EmailTemplatesResponse { Templates = templates };
        }

        templates = await ReadAsync(request.CultureName, request.TemplateName);

        SetCache(templates);

        return new EmailTemplatesResponse { Templates = templates };
    }


    protected abstract EmailTemplateSettings? TryGetFromCache(string cultureName, string templateName);

    protected abstract Task<EmailTemplateSettings?> ReadAsync(string cultureName, string templateName);

    protected abstract void SetCache(EmailTemplateSettings? template);

}
