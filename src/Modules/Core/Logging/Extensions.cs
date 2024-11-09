using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Modular.Ecommerce.Core.Logging.Enrichment;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

namespace Modular.Ecommerce.Core.Logging;

public class SerilogOptions
{
    internal static readonly Dictionary<string, string> DefaultOverride = new() { { "Microsoft.AspNetCore", "Warning" } };
    public bool ConsoleEnabled { get; set; } = true;
    public string MinimumLevel { get; set; } = "Information";
    public string Format { get; set; } = "compact";
    public Dictionary<string, string>? Override { get; set; } = DefaultOverride;

    public LogEventLevel GetMinimumLogEventLevel()
    {
        if (!Enum.TryParse<LogEventLevel>(MinimumLevel, true, out var level))
        {
            level = LogEventLevel.Information;
        }
        return level;
    }

    public static SerilogOptions Empty => new();
}


public static class Extensionss
{
    private static Dictionary<string, string> BindOverride(Dictionary<string, string>? o)
    {
        if (o is null)
        {
            return SerilogOptions.DefaultOverride;
        }
        foreach (var (key, value) in SerilogOptions.DefaultOverride)
        {
            o.TryAdd(key, value);
        }
        return o;
    }

    private static LogEventLevel ParseLevel(string? level)
    {
        return Enum.TryParse<LogEventLevel>(level, true, out var lvl) ? lvl : LogEventLevel.Information;
    }

    public static WebApplicationBuilder UseLogging(this WebApplicationBuilder builder, string? applicationName = null)
    {
        builder.Host.UseLogging(applicationName);
        return builder;
    }

    private static IHostBuilder UseLogging(this IHostBuilder hostBuilder, string? applicationName = null)
    {
        string? appName = applicationName ?? Assembly.GetExecutingAssembly().FullName;
        return hostBuilder.UseSerilog((context, configuration) =>
        {
            configuration
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("ApplicationName", appName)
                .Enrich.WithCustomerId()
                .Enrich.WithSpan()
                .ReadFrom.Configuration(context.Configuration);
        });
    }
}