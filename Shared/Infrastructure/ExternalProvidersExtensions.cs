using Hampcoders.Electrolink.API.Planning.Application.Internal.CommandServices;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Planning.Infrastructure.ExternalProviders;
using Hampcoders.Electrolink.API.Shared.Infrastructure.ExternalProviders;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Interfaces;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Interfaces.ASP.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Services;
using Hampcoders.Electrolink.API.Subscriptions.Infrastructure.PaymentGateway.StripeProvider;

namespace Hampcoders.Electrolink.API.Shared.Infrastructure;

public static class ExternalProvidersExtensions
{
    public static IServiceCollection AddExternalProviders(this IServiceCollection services, IConfiguration configuration)
    {
        AddPaymentProvider(services, configuration);
        AddFileStorageProvider(services, configuration);
        AddAIMatchingProvider(services, configuration);
        AddFileStorageService(services);
        return services;
    }

    private static void AddPaymentProvider(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["ExternalProviders:Payment"] ?? "stripe";
        switch (provider.ToLowerInvariant())
        {
            case "stripe":
                services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
                services.AddScoped<IPaymentProvider, StripePaymentProvider>();
                break;
            case "disabled":
                services.AddScoped<IPaymentProvider, NullPaymentProvider>();
                break;
            default:
                throw new InvalidOperationException(
                    $"Unknown payment provider: '{provider}'. Supported: stripe, disabled");
        }
    }

    private static void AddFileStorageProvider(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["ExternalProviders:FileStorage"] ?? "cloudinary";
        switch (provider.ToLowerInvariant())
        {
            case "cloudinary":
                services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
                services.AddScoped<IFileStorageProvider, CloudinaryFileStorageProvider>();
                break;
            case "disabled":
                services.AddScoped<IFileStorageProvider, NullFileStorageProvider>();
                break;
            default:
                throw new InvalidOperationException(
                    $"Unknown file storage provider: '{provider}'. Supported: cloudinary, disabled");
        }
    }

    private static void AddAIMatchingProvider(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["ExternalProviders:AIMatching"] ?? "disabled";
        switch (provider.ToLowerInvariant())
        {
            case "openai":
                services.Configure<OpenAISettings>(configuration.GetSection("OpenAI"));
                services.AddScoped<IAIMatchingProvider, OpenAIMatchingProvider>();
                break;
            case "gemini":
                services.Configure<GeminiSettings>(configuration.GetSection("Gemini"));
                services.AddHttpClient();
                services.AddScoped<IAIMatchingProvider, GeminiMatchingProvider>();
                break;
            case "disabled":
                services.AddScoped<IAIMatchingProvider, NullAIMatchingProvider>();
                break;
            default:
                throw new InvalidOperationException(
                    "Unknown AI matching provider: '{provider}'. Supported: openai, gemini, disabled");
        }

        services.AddScoped<IMatchingService, HybridMatchingService>();
    }

    private static void AddFileStorageService(IServiceCollection services)
    {
        services.AddScoped<IFileStorageService, FileStorageService>();
    }
}
