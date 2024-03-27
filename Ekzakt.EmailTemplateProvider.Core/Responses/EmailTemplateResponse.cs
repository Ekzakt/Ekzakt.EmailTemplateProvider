using Ekzakt.EmailTemplateProvider.Core.Models;

namespace Ekzakt.EmailTemplateProvider.Core.Responses;

public class EmailTemplateResponse
{
    public EmailTemplateSettings? Templates { get; set; }

    public bool IsSuccess => 
        Templates is not null && 
        Templates.EmailSettings is not null &&
        Templates.EmailSettings.Count > 0;
}
