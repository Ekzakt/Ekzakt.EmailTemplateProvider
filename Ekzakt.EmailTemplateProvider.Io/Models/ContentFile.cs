namespace Ekzakt.EmailTemplateProvider.Io.Models;

/// <summary>
/// This class represents the text-content of a file. Thechnically
/// this can be any type of text-file, but in this proejct they represent 
/// html, txt and json files.
/// </summary>
internal class ContentFile
{
    public string? Content { get; init; } = string.Empty;

    public ContentFile(string? content)
    {
        Content = content;
    }

    public long Size => Content?.Length ?? 0;
}
