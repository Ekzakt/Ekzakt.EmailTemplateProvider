using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;

namespace Ekzakt.EmailTemplateProvider.Io.Services;

public interface ITemplateFileReader
{
    Task<string?> ReadFileAsync(string filename, params string[] paths);

    Task<T?> ReadAsync<T>(string fileName, params string[] paths) where T : class?;

    Task<EmailTemplateSettings?> ReadSettingsFileAsync(EmailTemplateRequest request);
}
