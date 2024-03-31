using Ekzakt.EmailTemplateProvider.Core.Models;

namespace Ekzakt.EmailTemplateProvider.Core.Responses;

public class EmailTemplateResponse
{
    public EmailTemplateResponse(EmailTemplateSettings? emailTemplateSettings)
    {
        Templates = emailTemplateSettings;
    }


    public EmailTemplateSettings? Templates { get; init; }


    public bool IsSuccess => 
        Templates is not null && 
        Templates.EmailSettings is not null &&
        Templates.EmailSettings.Count > 0;
}
