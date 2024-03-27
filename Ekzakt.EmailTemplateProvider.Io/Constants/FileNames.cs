namespace Ekzakt.EmailTemplateProvider.Io.Constants;

internal static class FileRootNames
{
    public static string BODY(string recipientType) => $"{recipientType}.body";

    public const string SETTINGS = $"settings";

    public const string BASE = "base";

    public const string HEADER = "header";

    public const string FOOTER = "footer";
}