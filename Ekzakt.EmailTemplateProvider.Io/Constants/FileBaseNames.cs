namespace Ekzakt.EmailTemplateProvider.Io.Constants;

internal static class FileBaseNames
{
    public static string JSON_TEMPLATE_FILE(string recipientType) => $"{recipientType}.template.{FileTypes.JSON}".ToLower();

    public static string BODY_FILE(string recipientType) => $"{recipientType}.body";

    public static string BASE_FILE => "template.base";

    public static string HEADER_FILE => "template.header";

    public static string FOOTER_FILE => "template.footer";
}
