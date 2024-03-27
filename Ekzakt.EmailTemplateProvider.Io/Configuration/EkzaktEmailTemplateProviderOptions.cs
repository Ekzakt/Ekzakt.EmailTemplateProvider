namespace Ekzakt.EmailTemplateProvider.Io.Configuration;

public class EkzaktEmailTemplateProviderOptions
{
    public const string OptionsName = "Ekzakt:EmailTemplateProvider";

    public string BaseLocation { get; init; } = string.Empty;

    public double CacheSlidingExpiration { get; init; } = 3600;

    public string FallbackCultureName { get; init; } = "en-US";
}
