using Azure.Core;
using Ekzakt.EmailTemplateProvider.Core.Constants;
using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.EmailTemplateProvider.Core.Responses;
using Ekzakt.EmailTemplateProvider.Io.Configuration;
using Ekzakt.EmailTemplateProvider.Io.Constants;
using Ekzakt.EmailTemplateProvider.Io.Models;
using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.FileManager.Core.Models.Requests;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace Ekzakt.EmailTemplateProvider.Io.Services;

public class EkzaktEmailTemplateProviderIo(
    ILogger<EkzaktEmailTemplateProviderIo> logger,
    IOptions<EkzaktEmailTemplateProviderOptions> options,
    IFileManager fileManager,
    IMemoryCache memoryCache) : IEkzaktEmailTemplateProvider
{
    private readonly ILogger<EkzaktEmailTemplateProviderIo> _logger = logger;
    private readonly EkzaktEmailTemplateProviderOptions _options = options.Value;
    private readonly IFileManager _fileManager = fileManager;
    private readonly IMemoryCache _memoryCache = memoryCache;


    /// <summary>
    /// This method returns a list of EmailTemplates from an IO-based file system.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<EmailTemplateResponse> GetTemplateAsync(EmailTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var output = new List<EmailTemplate>();

        if (_memoryCache.TryGetValue(request.CacheKey, out List<EmailTemplate>? templates))
        {
            _logger.LogInformation("Retreiving {EmailTemplateCount} EmailTemplate(s) from cache with key '{CacheKey}'.", templates?.Count ?? 0, request.CacheKey);

            return new EmailTemplateResponse { Templates = templates };
        }

        var templateExists = await TemplateExistsAsync(request, cancellationToken);

        if (!templateExists)
        {
            return new EmailTemplateResponse();
        }

        var basePart = await GetTemplateBodyPartAsync(FileBaseNames.BASE_FILE);
        var headerPart = await GetTemplateBodyPartAsync(FileBaseNames.HEADER_FILE, request.TenantId ?? string.Empty, request.CultureName);
        var footerPart = await GetTemplateBodyPartAsync(FileBaseNames.FOOTER_FILE, request.TenantId ?? string.Empty, request.CultureName);

        foreach (var recipientType in RecipientTypes.TYPES)
        {
            var emailTemplate = await GetEmailTemplateFromJsonContentFile(request, recipientType);

            if (emailTemplate is null)
            {
                break;
            }

            var bodyPart = await GetTemplateBodyPartAsync(FileBaseNames.BODY_FILE(recipientType), request.TenantId ?? string.Empty, request.CultureName, request.TemplateName);

            emailTemplate.RecipientType = recipientType;

            ComposeTemplateBodyParts(
                emailTemplate: ref emailTemplate,
                basePart: basePart,
                headerPart: headerPart,
                bodyPart: bodyPart,
                footerPart: footerPart);

            output.Add(emailTemplate);
        }

        if (output.Count > 0)
        { 
            _logger.LogDebug("Adding EmailTeplates to cache with key '{CacheKey}'.", request.CacheKey);
            _memoryCache.Set(request.CacheKey, output, TimeSpan.FromSeconds(_options.CacheSlidingExpiration));
        }

        return new EmailTemplateResponse { Templates = output };
    }


    #region Helpers

    /// <summary>
    /// This method reads the body part content from their respective body 
    /// types: HTML and text.
    /// </summary>
    /// <param name="fileBaseName"></param>
    /// <param name="paths"></param>
    /// <returns></returns>
    internal async Task<TemplateBodyPart> GetTemplateBodyPartAsync(string fileBaseName, params string[] paths)
    {
        var bodyPart = new TemplateBodyPart();

        var contentHtml = await GetContentFileAsync($"{fileBaseName}.{FileTypes.HTML}", paths);
        bodyPart.Html = contentHtml?.Content ?? string.Empty;

        var contentText = await GetContentFileAsync($"{fileBaseName}.{FileTypes.TEXT}", paths);
        bodyPart.Text = contentText?.Content ?? string.Empty;

        return bodyPart;
    }


    /// <summary>
    /// This method composes the email template based on four different 
    /// body parts: the base part, the header part, the body part, and the footer part.
    /// </summary>
    /// <param name="emailTemplate"></param>
    /// <param name="basePart"></param>
    /// <param name="headerPart"></param>
    /// <param name="bodyPart"></param>
    /// <param name="footerPart"></param>
    internal void ComposeTemplateBodyParts(ref EmailTemplate emailTemplate, TemplateBodyPart? basePart, TemplateBodyPart? headerPart, TemplateBodyPart? bodyPart, TemplateBodyPart? footerPart)
    {
        var html = ComposeTemplateBodyPart(basePart?.Html, headerPart?.Html, bodyPart?.Html, footerPart?.Html);
        emailTemplate.Body.Html = html ?? string.Empty;

        var text = ComposeTemplateBodyPart(basePart?.Text, headerPart?.Text, bodyPart?.Text, footerPart?.Text);
        emailTemplate.Body.Text = text ?? string.Empty;
    }


    /// <summary>
    /// This method composes the actual email body, both for 
    /// </summary>
    /// <param name="basePart"></param>
    /// <param name="headerPart"></param>
    /// <param name="bodyPart"></param>
    /// <param name="footerPart"></param>
    /// <returns></returns>
    internal string? ComposeTemplateBodyPart(string? basePart, string? headerPart, string? bodyPart, string? footerPart)
    {
        var body = basePart ?? string.Empty;

        body = body?.Replace(Replacements.HEADER, headerPart ?? string.Empty);
        body = body?.Replace(Replacements.BODY, bodyPart ?? string.Empty);
        body = body?.Replace(Replacements.FOOTER, footerPart ?? string.Empty);

        return body;
    }


    /// <summary>
    /// This method returns an EmailTemplate object read the json file
    /// recipienttype.template.json.  If the file could not be found, no 
    /// EmailTemplate object for this recipient type will be created.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="recipientType"></param>
    /// <returns>EmailTemplate?</returns>
    internal async Task<EmailTemplate?> GetEmailTemplateFromJsonContentFile(EmailTemplateRequest request, string recipientType)
    {
        var fileName = FileBaseNames.JSON_TEMPLATE_FILE(recipientType);

        var jsonContentFile = await GetContentFileAsync(
            fileName,
            request.TenantId ?? string.Empty,
            request.CultureName,
            request.TemplateName);

        if (jsonContentFile is null)
        {
            _logger.LogDebug("{FileName} could not be found.  No EmailTemplate will be create for recipient type {RecipientType}.", fileName, recipientType);

            return null;
        }

        var output = JsonSerializer.Deserialize<EmailTemplate>(jsonContentFile?.Content ?? string.Empty);

        return output;
    }


    /// <summary>
    /// This method returns a ContentFile object if the file exists. 
    /// If not, this method returns null.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="paths"></param>
    /// <returns>ContentFile?</returns>
    internal async Task<ContentFile?> GetContentFileAsync(string fileName, params string[] paths)
    {
        var contentFile = await _fileManager.ReadFileStringAsync(new ReadFileAsStringRequest
        {
            BaseLocation = _options.BaseLocation,
            Paths = paths.ToList(),
            FileName = fileName.ToLower()
        });

        if (!contentFile.IsSuccess())
        {
            _logger.LogWarning("The file {FileName} could not be found.", fileName);

            return null;
        }

        return new ContentFile(contentFile.Data);
    }


    /// <summary>
    /// This method checks if the path for the template with
    /// the given CultureName and TemplateName exists.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<bool> TemplateExistsAsync(EmailTemplateRequest request, CancellationToken cancellationToken)
    {
        var listFilesRequest = new ListFilesRequest
        {
            BaseLocation = _options.BaseLocation,
            Paths = { request.CultureName, request.TemplateName}
        };

        var result = await _fileManager.ListFilesAsync(listFilesRequest, cancellationToken);

        if (result.Status == HttpStatusCode.OK)
        {
            return true;
        }

        _logger.LogWarning("The EmailTemplate '{TemplateName}' for culture '{CultureName}' could not be found. No EmailTemplate(s) is/are returned. Please be aware that Azure blobnames are case sensitive. ", request.TemplateName, request.CultureName);

        return false;
    }


    #endregion Helpers
}
