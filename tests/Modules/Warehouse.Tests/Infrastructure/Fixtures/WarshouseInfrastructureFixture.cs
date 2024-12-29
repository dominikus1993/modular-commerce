using Marten;
using Testcontainers.PostgreSql;
using Warehouse.Core.Repository;
using Warehouse.Infrastructure.Repository;

namespace Warehouse.Tests.Infrastructure.Fixtures;

public sealed class WarshouseInfrastructureFixture : IAsyncLifetime
{
    private PostgreSqlContainer PostgreSqlContainer = new PostgreSqlBuilder().Build();
    internal DocumentStore DocumentStore { get; private set; }
    
    public async Task InitializeAsync()
    {
        await PostgreSqlContainer.StartAsync();
        DocumentStore = DocumentStore.For(options =>
        {
            Setup.ConfigureMarten(options, PostgreSqlContainer.GetConnectionString());
        });
    }
    
    public ICurrentWarehouseStateReader CreateCurrentWarehouseStateReader()
    {
        return new CurrentWarehouseStateReader(DocumentStore);
    }

    public async Task DisposeAsync()
    {
        await DocumentStore.DisposeAsync();
        await PostgreSqlContainer.StopAsync();
    }
}