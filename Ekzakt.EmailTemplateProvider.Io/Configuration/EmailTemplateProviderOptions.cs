namespace Ekzakt.EmailTemplateProvider.Io.Configuration;

public class EmailTemplateProviderOptions
{
    public const string OptionsName = "Ekzakt:EmailTemplateProvider";

    public string[] RecipientTypes { get; init; } = [];

    public string BaseLocation { get; init; } = string.Empty;

    public double CacheSlidingExpiration { get; init; } = 3600;

    public string FallbackCultureName { get; init; } = "en-US";
}
