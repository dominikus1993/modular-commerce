using Serilog;
using Serilog.Configuration;

namespace Modular.Ecommerce.Core.Logging.Enrichment;

internal static class SerilogEnrichmentExtensions
{
    public static LoggerConfiguration WithCustomerId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        ArgumentNullException.ThrowIfNull(enrichmentConfiguration, nameof(enrichmentConfiguration));
        return enrichmentConfiguration.With<UserIdEnricher>();
    }
}