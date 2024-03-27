using Ekzakt.EmailTemplateProvider.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ekzakt.EmailTemplateProvider.Core.Caching;

public sealed class EmailTemplateCache(
    IMemoryCache memoryCache,
    ILogger<EmailTemplateCache> logger) : IEmailTemplateCache
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly ILogger<EmailTemplateCache> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public EmailTemplateSettings? GetTemplate(string cultureName, string templateName, string falbackCultureName)
    {
        if (TryGetValue(templateName, cultureName, out EmailTemplateSettings? template))
        {
            return template;
        }

        _logger.LogDebug("Trying to get '{CacheItem}' with fallback culture name {CultureName}.", nameof(EmailTemplateSettings), falbackCultureName);

        if (TryGetValue(templateName, falbackCultureName, out EmailTemplateSettings? fallbackTemplate))
        {
            return fallbackTemplate;
        }

        return null;
    }

    public void SetTemplate(EmailTemplateSettings emailTemplateSettings)
    {
        var keyName = GetCacheKeyName(emailTemplateSettings.CultureName, emailTemplateSettings.TemplateName);

        _logger.LogDebug("Setting cache value '{CacheKey}'", keyName);

        _memoryCache.Set(keyName, emailTemplateSettings);
    }

    #region Helpers

    private bool TryGetValue(string cultureName, string templateName, out EmailTemplateSettings? emailTemplate)
    {
        var key = GetCacheKeyName(cultureName, templateName);

        _logger.LogDebug("Attempting to read '{CacheItem}' from cache with key '{CacheKey}'.", nameof(EmailTemplateSettings), key);

        if (_memoryCache.TryGetValue(key, out EmailTemplateSettings? template))
        {
            _logger.LogDebug("'{CacheItem}' successfully read from cache with key '{CacheKey}'.", nameof(EmailTemplateSettings), key);

            emailTemplate = template;
            return true;
        }

        _logger.LogDebug("'{CacheItem}' does not exist in cache with key '{CacheKey}'.", nameof(EmailTemplateSettings), key);

        emailTemplate = null;

        return false;
    }


    private string GetCacheKeyName(string cultureName, string templateName)
    {
        return $"{cultureName}.{templateName}";
    }

    #endregion Helpers
}
