using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.EmailTemplateProvider.Core.Responses;

namespace Ekzakt.EmailTemplateProvider.Core.Contracts;

public interface IEkzaktEmailTemplateProvider
{
    Task<EmailTemplateResponse> GetTemplateAsync(EmailTemplateRequest request, CancellationToken cancellationToken = default);
}
