using AtlasBuho.Application.AiReview;
using AtlasBuho.Application.Translation;
using AtlasBuho.Data;
using AtlasBuho.Data.Translation;
using AtlasBuho.Infrastructure.AiProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
        // Register with Options pattern so IOptions<AiReviewOptions> resolves correctly
        services.AddOptions<AiReviewOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                var section = config.GetSection("AtlasBuho:AI");
                section.Bind(options);
            });

        // AI reviewer (production: external provider, tests: MockAiTranslationReviewer)
        // Provider selection through configuration (B10.1 §3)
        services.AddScoped<IAiTranslationReviewer>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<AiReviewOptions>>();
            var aiOptions = options.Value;

            // Validate configuration
            AiReviewOptionsValidator.Validate(aiOptions);

            return aiOptions.Provider?.ToLowerInvariant() switch
            {
                "mock" => new MockAiTranslationReviewer(aiOptions),
                "disabled" => new DisabledAiTranslationReviewer(),
                null => new DisabledAiTranslationReviewer(), // Default to disabled if not specified
                "" => new DisabledAiTranslationReviewer(),  // Empty string also means disabled
                _ => throw new InvalidOperationException(
                    $"Unknown AI provider: '{aiOptions.Provider}'. " +
                    $"Valid providers: Mock, Disabled. " +
                    $"External providers must be registered separately.")
            };
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
        // Register with Options pattern so IOptions<AiReviewOptions> resolves correctly
        services.AddOptions<AiReviewOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                var section = config.GetSection("AtlasBuho:AI");
                section.Bind(options);
            });

        // AI reviewer — same provider selection logic as production (IOptions validation applies)
        services.AddScoped<IAiTranslationReviewer>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<AiReviewOptions>>();
            var aiOptions = options.Value;

            AiReviewOptionsValidator.Validate(aiOptions);

            return aiOptions.Provider?.ToLowerInvariant() switch
            {
                "mock" => new MockAiTranslationReviewer(aiOptions),
                "disabled" => new DisabledAiTranslationReviewer(),
                null => new DisabledAiTranslationReviewer(),
                "" => new DisabledAiTranslationReviewer(),
                _ => throw new InvalidOperationException(
                    $"Unknown AI provider: '{aiOptions.Provider}'. " +
                    $"Valid providers: Mock, Disabled. " +
                    $"External providers must be registered separately.")
            };
        });

        // Translation orchestrator
        services.AddScoped<TranslationQueryHandler>();

        return services;
    }
}