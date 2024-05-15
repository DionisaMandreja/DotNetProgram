namespace DotNetProgram.Tests;

public class UnitTest1
{
    private readonly Mock<CosmosClient> _mockCosmosClient;
    private readonly Mock<Container> _mockContainer;
    private readonly CosmosDbService _service;
    private readonly string _databaseId = "SampleDB";
    private readonly string _containerId = "MainPrograms";

    public UnitTest1()
    {
   
        _mockCosmosClient = new Mock<CosmosClient>();

   
        _mockContainer = new Mock<Container>();
        _mockCosmosClient.Setup(c => c.GetContainer(_databaseId, _containerId))
                         .Returns(_mockContainer.Object);

   
        _service = new CosmosDbService(_mockCosmosClient.Object, _databaseId);
    }

    [Fact]
    public async Task AddItemAsync_CallsCreateItemAsync()
    {
        var testItem = new { Id = "1", Name = "Test Item" };
        _mockContainer.Setup(x => x.CreateItemAsync(
            It.IsAny<object>(),
            It.IsAny<PartitionKey>(),
            null,
            default))
        .ReturnsAsync(Response.FromValue(testItem, new ResponseMessage(System.Net.HttpStatusCode.OK)));

        await _service.AddItemAsync(testItem, _containerId, "1");

        _mockContainer.Verify(x => x.CreateItemAsync(
            It.Is<object>(i => i == testItem),
            It.IsAny<PartitionKey>(),
            null,
            default), Times.Once);
    }
}