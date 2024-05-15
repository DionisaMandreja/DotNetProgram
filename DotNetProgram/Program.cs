using dotnetProgram;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
var connectionString = cosmosDbConfig["ConnectionString"];
var cosmosClient = new CosmosClient(connectionString);
var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbConfig["DatabaseId"]);

async Task<Container> CreateContainerAsync(Database database, string containerId, string partitionKeyPath)
{
    ContainerProperties containerProperties = new ContainerProperties
    {
        Id = containerId,
        PartitionKeyPath = partitionKeyPath
    };

    try
    {
        Container container = database.GetContainer(containerId);
        await container.ReadContainerAsync(); 
        return container;
    }
    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
       
        return await database.CreateContainerAsync(containerProperties);
    }
}
var mainProgramContainer = await CreateContainerAsync(database, "MainPrograms", "/id");
builder.Services.AddSingleton<CosmosDbService>(new CosmosDbService(cosmosClient, cosmosDbConfig["DatabaseId"]));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
