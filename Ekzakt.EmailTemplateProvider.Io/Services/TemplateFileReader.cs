using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.EmailTemplateProvider.Io.Configuration;
using Ekzakt.EmailTemplateProvider.Io.Constants;
using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.FileManager.Core.Models.Requests;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Ekzakt.EmailTemplateProvider.Io.Services;

internal class TemplateFileReader : ITemplateFileReader
{
    private readonly IFileManager _fileManager;
    private readonly EkzaktEmailTemplateProviderOptions _options;


    public TemplateFileReader(
        IFileManager fileManager,
        IOptions<EkzaktEmailTemplateProviderOptions> options)
    {
        _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }


    public async Task<EmailTemplateSettings?> ReadSettingsFileAsync(EmailTemplateRequest request)
    {
        var fileName = $"{FileRootNames.SETTINGS}.{FileTypes.JSON}";

        return await ReadAsync<EmailTemplateSettings>(fileName, request.TenantId ?? string.Empty, request.CultureName, request.TemplateName);
    }


    public async Task<string?> ReadFileAsync(string filename, params string[] paths)
    {
        var request = new ReadFileAsStringRequest
        {
            BaseLocation = _options.BaseLocation,
            Paths = paths.ToList(),
            FileName = filename,
        };

        var response = await _fileManager.ReadFileStringAsync(request);

        if (response.IsSuccess())
        {
            return response.Data;
        }

        return null;
    }


    public async Task<T?> ReadAsync<T>(string fileName, params string[] paths)
        where T : class?
    {
        var content = await ReadFileAsync(fileName, paths);

        if (content is null)
        {
            return null;
        }

        var result = JsonSerializer.Deserialize<T>(content);

        return result;
    }
}
