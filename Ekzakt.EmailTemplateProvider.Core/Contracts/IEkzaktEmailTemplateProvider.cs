using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.EmailTemplateProvider.Core.Responses;

namespace Ekzakt.EmailTemplateProvider.Core.Contracts;

public interface IEkzaktEmailTemplateProvider
{
    Task<EmailTemplatesResponse> GetEmailTemplatesAsync(EmailTemplatesRequest request, CancellationToken cancellationToken = default);
}
