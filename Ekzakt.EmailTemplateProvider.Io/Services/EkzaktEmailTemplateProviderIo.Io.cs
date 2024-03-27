using Ekzakt.EmailTemplateProvider.Core.Caching;
using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Io.Configuration;
using Ekzakt.EmailTemplateProvider.Io.Constants;
using Ekzakt.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ekzakt.EmailTemplateProvider.Io.Services;

public class EkzaktEmailTemplateProviderIo(
    ILogger<EkzaktEmailTemplateProviderIo> logger,
    IOptions<EkzaktEmailTemplateProviderOptions> options,
    IEmailTemplateCache cache,
    FileReader fileReader) : AbstractEmailTemplateProvider
{
    private readonly ILogger<EkzaktEmailTemplateProviderIo> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    private readonly EkzaktEmailTemplateProviderOptions _options = options?.Value
        ?? throw new ArgumentNullException(nameof(options));

    private readonly IEmailTemplateCache _cache = cache
        ?? throw new ArgumentNullException(nameof(cache));

    private readonly FileReader _fileReader = fileReader 
        ?? throw new ArgumentNullException(nameof(fileReader));


    protected override EmailTemplateSettings? TryGetFromCache(string cultureName, string templateName)
    {
        return _cache.GetTemplate(cultureName, templateName, _options.FallbackCultureName);
    }


    protected override async Task<EmailTemplateSettings?> ReadAsync(string cultureName, string templateName)
    {
        var settings = await ReadSettingsFileAsync(cultureName, templateName);

        if (settings is null || !IsSettingsValid(settings, cultureName, templateName))
        {
            return null;
        }

        var (baseHtml, baseText) = await ReadFilesAsync(FileRootNames.BASE);
        var (headerHtml, headerText) = await ReadFilesAsync(FileRootNames.HEADER, cultureName);
        var (footerHtml, footerText) = await ReadFilesAsync(FileRootNames.FOOTER, cultureName);

        foreach (var setting in settings.EmailSettings ?? [])
        {
            var (bodyHtml, bodyText) = await ReadFilesAsync(FileRootNames.BODY(setting.RecipientType), cultureName, templateName);

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


    protected override void SetCache(EmailTemplateSettings? template)
    {
        if (template is null)
        {
            return;
        }

        _cache.SetTemplate(template);
    }


    #region Helpers

    private bool IsSettingsValid(EmailTemplateSettings settings, string cultureName, string templateName)
    {
        return settings.IsValid && settings.CultureName == cultureName && settings.TemplateName == templateName;
    }

    private async Task<(string?, string?)> ReadFilesAsync(string fileRootName, params string[] paths)
    {
        var html = await ReadFileAsync($"{fileRootName}.{FileTypes.HTML}", paths);
        var text = await ReadFileAsync($"{fileRootName}.{FileTypes.TEXT}", paths);

        return (html, text);
    }

    private async Task<string?> ReadFileAsync(string fileName, params string[] parameters)
    {
        return await _fileReader.ReadAsync(fileName, parameters);
    }

    private async Task<EmailTemplateSettings?> ReadSettingsFileAsync(string cultureName, string templateName)
    {
        var fileName = $"{FileRootNames.SETTINGS}.{FileTypes.JSON}";

        return await _fileReader.ReadAsync<EmailTemplateSettings>(fileName, cultureName, templateName);
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

    #endregion Helpers
}
