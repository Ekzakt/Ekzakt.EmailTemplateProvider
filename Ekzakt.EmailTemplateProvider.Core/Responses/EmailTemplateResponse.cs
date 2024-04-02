using Ekzakt.EmailTemplateProvider.Core.Models;

namespace Ekzakt.EmailTemplateProvider.Core.Responses;

public class EmailTemplateResponse
{
    public EmailTemplateResponse(EmailTemplateInfo? emailTemplateInfo)
    {
        EmailTemplateInfo = emailTemplateInfo;
    }


    public EmailTemplateInfo? EmailTemplateInfo { get; init; }


    public bool IsSuccess => 
        EmailTemplateInfo is not null && 
        EmailTemplateInfo.EmailInfos is not null &&
        EmailTemplateInfo.EmailInfos.Count > 0;
}
