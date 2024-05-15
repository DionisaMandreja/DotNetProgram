using dotnetProgram.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace dotnetProgram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramController:ControllerBase
    {
            private readonly CosmosDbService _cosmosDbService;
            private readonly string _containerId = "MainPrograms"; // Specify the container ID for Programs
            private readonly string partitionKey = "/id";
            public ProgramController(CosmosDbService cosmosDbService)
            {
                _cosmosDbService = cosmosDbService;
            }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplication(string id)
        {
            var application = await _cosmosDbService.GetItemAsync<Applications>(id, "Applications", id);
            if (application == null)
                return NotFound();

            return Ok(application);
        }

        //Provide a POST endpoint for the front-end team with the payload structure so that once
        //the candidate submits the application, you will receive the data
        [HttpPost]
        public async Task<IActionResult> SubmitApplication([FromBody] Applications application)
        {
            if (application == null)
                return BadRequest("Application data is required.");


            await _cosmosDbService.AddItemAsync(application, "Applications", application.Id);
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }

    }

}
