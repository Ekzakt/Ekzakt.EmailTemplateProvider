using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Ekzakt.EmailTemplateProvider.Io.Services;

namespace Ekzakt.EmailTemplateProvider.Io.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddEmailTemplateProviderIo(this IServiceCollection services, Action<EmailTemplateProviderOptions> options)
    {
        services.Configure(options);

        services.AddEmailTemplateProviderIo();

        return services;
    }


    public static IServiceCollection AddEmailTemplateProviderIo(this IServiceCollection services, string? configSectionPath = null)
    {
        configSectionPath ??= EmailTemplateProviderOptions.OptionsName;

        services
            .AddOptions<EmailTemplateProviderOptions>()
            .BindConfiguration(configSectionPath);

        services.AddEmailTemplateProviderIo();

        return services;
    }




    #region Helpers

    private static IServiceCollection AddEmailTemplateProviderIo(this IServiceCollection services)
    {
        services.AddScoped<IEmailTemplateProvider, IoEmailTemplateProvider>();
        services.AddMemoryCache();

        return services;
    }

    #endregion Helpers
}
