using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.Utilities.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ekzakt.EmailTemplateProvider.Core.Caching;

public sealed class EmailTemplateCache(
    IMemoryCache memoryCache,
    ILogger<EmailTemplateCache> logger) : IEmailTemplateCache
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly ILogger<EmailTemplateCache> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<(bool, EmailTemplateInfo?)> TryGetTemplate(EmailTemplateRequest request, Func<EmailTemplateRequest, Task<EmailTemplateInfo?>> onCacheKeyNotFound)
    {
        if (_memoryCache.TryGetValue(request.CacheKeyName, out EmailTemplateInfo? template))
        {
            _logger.LogDebug("A value of {CacheKeyName} was found in cache and is being returned.", request.CacheKeyName);

            return (true, template);
        }

        _logger.LogDebug("No value of {CacheKeyName} was found in cache.", request.CacheKeyName);
        var output = await onCacheKeyNotFound(request);

        SetTemplate(request.CacheKeyName, output);

        return (true, output);
    }


    public void SetTemplate(string cacheKeyName, EmailTemplateInfo? emailTemplateInfo)
    {
        _logger.LogDebug("Setting cache value '{CacheKey}'.", cacheKeyName);

        _memoryCache.Set(cacheKeyName, emailTemplateInfo);
    }
}
