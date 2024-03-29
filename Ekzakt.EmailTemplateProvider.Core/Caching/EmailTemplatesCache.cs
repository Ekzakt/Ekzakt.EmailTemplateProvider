using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ekzakt.EmailTemplateProvider.Core.Caching;

public sealed class EmailTemplatesCache(
    IMemoryCache memoryCache,
    ILogger<EmailTemplatesCache> logger) : IEmailTemplatesCache
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly ILogger<EmailTemplatesCache> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public event IEmailTemplatesCache.AsyncEventHandler<CacheKeyNotFoundEventArgs>? CacheKeyNotFound;

    public async Task<EmailTemplateSettings?> GetTemplateAsync(EmailTemplatesRequest request, string? fallbackCultureName = null)
    {
        if (TryGetValue(request, out EmailTemplateSettings? template))
        {
            if (template is not null && template.IsValid)
            {
                return template;
            }

            if (!string.IsNullOrEmpty(fallbackCultureName))
            {
                request.CultureName = fallbackCultureName;
                return await GetTemplateAsync(request);
            }
        }

        throw new InvalidOperationException("dsfmldsqkjfdsmlqfjqsdf");
    }


    public void SetTemplate(EmailTemplateSettings emailTemplateSettings)
    {
        var cacheKeyName = emailTemplateSettings.CacheKeyName;

        _logger.LogDebug("Setting cache value '{CacheKey}'", cacheKeyName);

        _memoryCache.Set(cacheKeyName, emailTemplateSettings);
    }


    #region Helpers

    private async bool TryGetValue(EmailTemplatesRequest request, out EmailTemplateSettings? emailTemplateSettings)
    {
        _logger.LogDebug("Attempting to read '{CacheItem}' from cache with key '{CacheKey}'.", nameof(EmailTemplateSettings), request.CacheKeyName);

        if (_memoryCache.TryGetValue(request.CacheKeyName, out EmailTemplateSettings? template))
        {
            _logger.LogDebug("'{CacheItem}' successfully read from cache with key '{CacheKey}'.", nameof(EmailTemplateSettings), request.CacheKeyName);

            emailTemplateSettings = template;
            return true;
        }

        _logger.LogDebug("'{CacheItem}' does not exist in cache with key '{CacheKey}'.", nameof(EmailTemplateSettings), request.CacheKeyName);

        var cacheKeyEventArgs = new CacheKeyNotFoundEventArgs
        {
            TenantId = request.TenantId,
            CultureName = request.CultureName,
            TemplateName = request.TemplateName,
        };

        await OnCacheKeyNotFoundAsync(cacheKeyEventArgs);

        emailTemplateSettings = null;
        return false;
    }


    private async Task OnCacheKeyNotFoundAsync(CacheKeyNotFoundEventArgs e)
    {
        if (CacheKeyNotFound is not null)
        {
            _logger.LogDebug("Sending event {0}.", nameof(CacheKeyNotFound));
            await CacheKeyNotFound(e);
        }
    }

    #endregion Helpers
}
