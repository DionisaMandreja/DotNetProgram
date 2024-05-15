namespace DotNetProgram.Tests;

public class UnitTest1
{
    private readonly Mock<CosmosClient> _mockCosmosClient;
    private readonly Mock<Container> _mockContainer;

    private readonly CosmosDbService _service;
    private readonly ProgramController programController; 

    private readonly string _databaseId = "SampleDB";
    private readonly string _containerId = "MainPrograms";

    public UnitTest1()
    {
   
        _mockCosmosClient = new Mock<CosmosClient>();

   
        _mockContainer = new Mock<Container>();
        _mockCosmosClient.Setup(c => c.GetContainer(_databaseId, _containerId))
                         .Returns(_mockContainer.Object);

   
        _service = new CosmosDbService(_mockCosmosClient.Object, _databaseId);
        programController = new ProgramController(_mockService.Object);
    }

    [Fact]
    public async Task GetApplication_NotFound()
    {
       
        var appId = "nonexistent";
        _mockService.Setup(s => s.GetItemAsync<Applications>(appId, "Applications", appId))
                    .ReturnsAsync((Applications)null);

        var result = await _controller.GetApplication(appId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetApplication_Found()
    {
        
        var appId = "existing";
        var application = new Applications { Id = appId };
        _mockService.Setup(s => s.GetItemAsync<Applications>(appId, "Applications", appId))
                    .ReturnsAsync(application);

        var result = await _controller.GetApplication(appId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(application, okResult.Value);
    }

    [Fact]
    public async Task SubmitApplication_BadRequest()
    {
        var result = await _controller.SubmitApplication(null);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task SubmitApplication_Created()
    {
        var application = new Applications { Id = "123" };
        _mockService.Setup(s => s.AddItemAsync(application, "Applications", application.Id))
                    .Returns(Task.CompletedTask);

        var result = await _controller.SubmitApplication(application);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ApplicationsController.GetApplication), createdAtActionResult.ActionName);
        Assert.Equal(application.Id, createdAtActionResult.RouteValues["id"]);
    }
}