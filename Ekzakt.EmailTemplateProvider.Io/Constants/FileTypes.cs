namespace Ekzakt.EmailTemplateProvider.Io.Constants;

internal class FileTypes
{
    public const string HTML = "html";

    public const string TEXT = "txt";

    public const string JSON = "json";

    /// <summary>
    /// These are the two filetypes of which a template-body
    /// can be composed. So, files for a HTML-body, and for
    /// a text-body.
    /// </summary>
    public static string[] TYPES = [ HTML, TEXT ];
}
