using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Ekzakt.EmailTemplateProvider.Io.Services;

namespace Ekzakt.EmailTemplateProvider.Io.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddEkzaktEmailTemplateProviderIo(this IServiceCollection services, Action<EkzaktEmailTemplateProviderOptions> options)
    {
        services.Configure(options);

        services.AddEkzaktEmailTemplateProviderIo();

        return services;
    }


    public static IServiceCollection AddEkzaktEmailTemplateProviderIo(this IServiceCollection services, string? configSectionPath = null)
    {
        configSectionPath ??= EkzaktEmailTemplateProviderOptions.OptionsName;

        services
            .AddOptions<EkzaktEmailTemplateProviderOptions>()
            .BindConfiguration(configSectionPath);

        services.AddEkzaktEmailTemplateProviderIo();

        return services;
    }




    #region Helpers

    private static IServiceCollection AddEkzaktEmailTemplateProviderIo(this IServiceCollection services)
    {
        services.AddScoped<IEkzaktEmailTemplateProvider, EkzaktEmailTemplateProviderIo>();
        services.AddMemoryCache();

        return services;
    }

    #endregion Helpers
}
