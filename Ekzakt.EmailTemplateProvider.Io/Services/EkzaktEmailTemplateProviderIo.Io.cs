using Ekzakt.EmailTemplateProvider.Core;
using Ekzakt.EmailTemplateProvider.Core.Caching;
using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.EmailTemplateProvider.Core.Responses;
using Ekzakt.EmailTemplateProvider.Io.Configuration;
using Ekzakt.EmailTemplateProvider.Io.Constants;
using Ekzakt.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ekzakt.EmailTemplateProvider.Io.Services;

public class EkzaktEmailTemplateProviderIo : IEkzaktEmailTemplateProvider
{
    private readonly ILogger<EkzaktEmailTemplateProviderIo> _logger;
    private readonly EkzaktEmailTemplateProviderOptions _options;
    private readonly IEmailTemplateCache _cache;
    private readonly ITemplateFileReader _fileReader;

    public EkzaktEmailTemplateProviderIo(
        ILogger<EkzaktEmailTemplateProviderIo> logger,
        IOptions<EkzaktEmailTemplateProviderOptions> options,
        IEmailTemplateCache cache,
        ITemplateFileReader fileReader)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
    }


    public async Task<EmailTemplateResponse?> GetEmailTemplateAsync(EmailTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var (isSuccess, template) = await _cache.TryGetTemplate(request, OnCacheKeyNotFound);

        if (isSuccess)
        {
            if (template is not null && template.IsValid)
            {
                return new EmailTemplateResponse(template);
            }

            if (!string.IsNullOrEmpty(_options.FallbackCultureName) && request.CultureName != _options.FallbackCultureName)
            {
                var fallbackRequest = new EmailTemplateRequest
                {
                    TenantId = request.TenantId,
                    CultureName = _options.FallbackCultureName.ToLower(),
                    TemplateName = request.TemplateName
                };

                return await GetEmailTemplateAsync(fallbackRequest, cancellationToken);
            }
        }


        if (_options.ThrowOnException)
        {
            throw new InvalidOperationException(
                $"A template with request {request} cound not be found, nor " +
                $"could a template with fallback culturename '{_options.FallbackCultureName}' be " +
                $"found. This is likely caused by the requested template name " +
                $"'{request.TemplateName}' being invalid.");
        }

        _logger.LogWarning(
            "A template with request '{EmailTemplateRequest}' cound not be found, nor " +
            "could a template with fallback culturename '{FallbackCultureName}' be " +
            "found. This is likely caused by the request template name " +
            "'{TemplateName}' being invalid.", request.ToString(), _options.FallbackCultureName.ToLower(), request.TemplateName);

        return null;
    }


    public async Task<EmailTemplateInfo?> ReadTemplateFilesAsync(EmailTemplateRequest request)
    {
        _logger.LogInformation("Reading templates {CacheKeyName} from file system.", request.CacheKeyName);

        var settings = await _fileReader.ReadSettingsFileAsync(request);

        if (settings is null || !IsSettingsValid(settings, request.TenantId, request.CultureName, request.TemplateName))
        {
            return null;
        }

        var (baseHtml, baseText) = await ReadBaseFilesAsync(FileRootNames.BASE);
        var (headerHtml, headerText) = await ReadBaseFilesAsync(FileRootNames.HEADER, request.TenantId ?? string.Empty, request.CultureName);
        var (footerHtml, footerText) = await ReadBaseFilesAsync(FileRootNames.FOOTER, request.TenantId ?? string.Empty, request.CultureName);

        foreach (var setting in settings.EmailInfos ?? [])
        {
            var (bodyHtml, bodyText) = await ReadBaseFilesAsync(FileRootNames.BODY(setting.RecipientType), request.TenantId ?? string.Empty, request.CultureName, request.TemplateName);

            if (setting.Email is not null)
            {
                var html = ComposeEmail(baseHtml, headerHtml, bodyHtml, footerHtml);
                setting.Email.Body.Html = html ?? string.Empty;

                var text = ComposeEmail(baseText, headerText, bodyText, footerText);
                setting.Email.Body.Text = text ?? string.Empty;
            }
        }

        return settings;
    }


    #region Helpers

    private bool IsSettingsValid(EmailTemplateInfo templateInfo, string? tenantId, string cultureName, string templateName)
    {
        return templateInfo.IsValid && 
            templateInfo.TenantId == tenantId &&
            templateInfo.CultureName == cultureName && 
            templateInfo.TemplateName == templateName;
    }


    private async Task<(string?, string?)> ReadBaseFilesAsync(string fileRootName, params string[] paths)
    {
        var html = await _fileReader.ReadFileAsync($"{fileRootName}.{FileTypes.HTML}", paths);
        var text = await _fileReader.ReadFileAsync($"{fileRootName}.{FileTypes.TEXT}", paths);

        return (html, text);
    }


    private string? ComposeEmail(string? baseContent, string? headerContent, string? bodyContent, string? footerContent)
    {
        if (baseContent is null)
        {
            _logger.LogInformation("{baseContent} is null. No content is returned.", nameof(baseContent));
            return null;
        }

        var replacer = new StringReplacer();

        replacer.AddReplacement(Replacements.HEADER, headerContent ?? string.Empty);
        replacer.AddReplacement(Replacements.BODY, bodyContent ?? string.Empty);
        replacer.AddReplacement(Replacements.FOOTER, footerContent ?? string.Empty);

        var output = replacer.Replace(baseContent);

        return output;
    }


    private async Task<EmailTemplateInfo?> OnCacheKeyNotFound(EmailTemplateRequest request)
    {
        _logger.LogDebug("Reading template from source with request value {EmailTemplateRequest}.", request.ToString());

        var templates = await ReadTemplateFilesAsync(request);

        return templates;
    }

    #endregion Helpers
}
