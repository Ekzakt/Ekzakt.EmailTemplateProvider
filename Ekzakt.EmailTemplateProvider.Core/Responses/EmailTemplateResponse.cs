using Ekzakt.EmailTemplateProvider.Core.Models;

namespace Ekzakt.EmailTemplateProvider.Core.Responses;

public class EmailTemplateResponse
{
    public List<EmailTemplate>? Templates { get; set; }

    public bool IsSuccess => Templates is not null && Templates.Count > 0;
}
