using JasperFx.CodeGeneration;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Warehouse.Core.Model;
using Warehouse.Infrastructure.Projections;
using Weasel.Core;

namespace Warehouse;

public static class Setup
{
    public static WebApplicationBuilder AddWarehouseModule(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddMarten(options => ConfigureMarten(options, builder.Configuration.GetConnectionString("Warehouse") ?? throw new InvalidOperationException("Warehouse connection string is missing")))
            .OptimizeArtifactWorkflow(TypeLoadMode.Static)
            .ApplyAllDatabaseChangesOnStartup()
            .UseLightweightSessions()
            .AddAsyncDaemon(DaemonMode.HotCold);
        
        return builder;
    }

    internal static void ConfigureMarten(StoreOptions options, string connectionString, string schema = "warehouse")
    {
        options.DatabaseSchemaName = schema;
        options.Connection(connectionString);
        options.UseSystemTextJsonForSerialization(EnumStorage.AsString);
        options.ApplicationAssembly = typeof(Setup).Assembly;
        
        options.Events.StreamIdentity = StreamIdentity.AsGuid;
        options.Events.MetadataConfig.HeadersEnabled = true;
        options.Events.MetadataConfig.CausationIdEnabled = true;
        options.Events.MetadataConfig.CorrelationIdEnabled = true;


        options.Projections.Errors.SkipApplyErrors = false;
        options.Projections.Errors.SkipSerializationErrors = false;
        options.Projections.Errors.SkipUnknownEvents = false;
        
        options.Projections.LiveStreamAggregation<WarehouseState>();
        options.Projections.Add<WarehouseStateProjection>(ProjectionLifecycle.Inline);
        options.Projections.Snapshot<WarehouseState>(SnapshotLifecycle.Inline);
        options.DisableNpgsqlLogging = true;
    }
}