using AtlasBuho.Application.AiReview;
using AtlasBuho.Application.Translation;
using AtlasBuho.Data;
using AtlasBuho.Data.Translation;
using AtlasBuho.Infrastructure.AiProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace AtlasBuho.Data.Extensions;

/// <summary>
/// Composition root for AtlasBuho translation services (B10 §14).
/// Registers the real executable path: TranslationQueryHandler → ITranslationEngine → IAiTranslationReviewer.
/// 
/// <para><b>Usage in a future host:</b></para>
/// <code>
/// services.AddAtlasBuhoTranslation("Host=...;Database=atlasbuho;...");
/// </code>
/// 
/// <para>Tests override IAiTranslationReviewer with MockAiTranslationReviewer.</para>
/// </summary>
public static class TranslationServiceCollectionExtensions
{
    /// <summary>
    /// Registers AtlasBuho translation services with real MySQL persistence.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">MySQL connection string</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddAtlasBuhoTranslation(
        this IServiceCollection services,
        string connectionString)
    {
        // Persistence
        services.AddDbContext<AtlasBuhoDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // V1 canonical translation engine
        services.AddScoped<ITranslationEngine, DictionaryTranslationEngine>();

        // AI orchestration persistence
        services.AddScoped<IAiOrchestrationPersistence, AiOrchestrationPersistence>();

        // Candidate extraction
        services.AddScoped<ICandidateExtractor, CandidateExtractor>();

        // AI review options (from configuration or defaults)
        services.AddSingleton(new AiReviewOptions());

        // AI reviewer (production: external provider, tests: MockAiTranslationReviewer)
        // Registered as optional — may be null if AI disabled in configuration
        services.AddScoped<IAiTranslationReviewer?>(provider =>
        {
            var options = provider.GetService<AiReviewOptions>();
            if (options?.Enabled == true)
            {
                // In real deployment: return configured provider implementation
                // For now, return null to indicate AI disabled (B10 §10)
                return null;
            }
            return null;
        });

        // Translation orchestrator
        services.AddScoped<TranslationQueryHandler>();

        return services;
    }

    /// <summary>
    /// Registers AtlasBuho translation services with InMemory database for testing.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="databaseName">In-memory database name</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddAtlasBuhoTranslationForTesting(
        this IServiceCollection services,
        string databaseName = "TestDb")
    {
        // Persistence (InMemory)
        services.AddDbContext<AtlasBuhoDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));

        // V1 canonical translation engine
        services.AddScoped<ITranslationEngine, DictionaryTranslationEngine>();

        // AI orchestration persistence
        services.AddScoped<IAiOrchestrationPersistence, AiOrchestrationPersistence>();

        // Candidate extraction
        services.AddScoped<ICandidateExtractor, CandidateExtractor>();

        // AI review options (disabled by default in tests unless overridden)
        services.AddSingleton(new AiReviewOptions { Enabled = false });

        // Translation orchestrator
        services.AddScoped<TranslationQueryHandler>();

        return services;
    }
}