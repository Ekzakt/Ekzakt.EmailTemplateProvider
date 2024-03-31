namespace Ekzakt.EmailTemplateProvider.Core.Responses;

public class EmailTemplateCacheResponse
{
    public List<(string, bool)>? CacheValues { get; set; }

    public int Count => CacheValues?.Count ?? 0; 
}
