using Modular.Ecommerce.Catalog;
using Modular.Ecommerce.Catalog.Api;
using Modular.Ecommerce.Core.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.UseLogging("ecommerce");
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddCatalog();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.InitializeCatalog();
app.MapCatalog();

await app.RunAsync();

public sealed partial class Program;