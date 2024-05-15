using dotnetProgram.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using System.Buffers.Text;
using System.Net;
using System.Runtime.Intrinsics.X86;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace dotnetProgram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController:ControllerBase
    {

        private readonly CosmosDbService _cosmosDbService;
        private readonly string _containerId = "MainPrograms"; // Specify the container ID for Programs
        private readonly string partitionKey = "/id";
        public QuestionController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(string id)
        {
            var question = await _cosmosDbService.GetItemAsync<Questions>(id, _containerId, id);
            if (question == null)
                return NotFound();
            return Ok(question);
        }
        // Use the POST method to correctly store the different types of questions we have
        //(Paragraph, YesNo, Dropdown, MultipleChoice, Date, Number)
        [HttpPost]
        public async Task<IActionResult> AddQuestions([FromBody] Questions question)
        {
            if (question == null)
                return BadRequest("Question data is required.");
            string containerId = "Questions";

            await _cosmosDbService.AddItemAsync(question, containerId, question.Id);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

        // an api to be called so the front end can have the list of question type fro the dropdown 
        //[HttpGet]
        //public IActionResult GetQuestionTypes()
        //{
        //    var questionTypes = new List<QuestionsType>
        //{
        //    new QuestionsType { Type = "Multiple choice" },
        //    new QuestionsType { Type = "Paragraph" },
        //    new QuestionsType { Type = "Yes/No" },
        //    new QuestionsType { Type = "Dropdown" },
        //    new QuestionsType { Type = "Date" },
        //    new QuestionsType { Type = "Number" }
        //};

        //    return Ok(questionTypes);
        //}
        // Use the PUT method if the user wants to edit the question after creating the application
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(string id, [FromBody] Questions question)
        {
            if (question == null || id != question.Id)
            {
                return BadRequest("Invalid question data.");
            }

            string containerId = "Questions";
            string partitionKeyValue = question.Id;

            try
            {
                await _cosmosDbService.UpdateItemAsync(id, question, containerId, partitionKeyValue);
                return NoContent();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound($"No question found with ID: {id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the question: {ex.Message}");
            }
        }


        //You should provide a GET endpoint for the front-end team to render the questions based
        //on the question type.
        [HttpGet]
        public async Task<IActionResult> GetQuestions(string type)
        {
            var query = $"SELECT * FROM c WHERE c.Type = '{type}'";
            var questions = await _cosmosDbService.QueryItemsAsync<Questions>(query, "Questions");
            return Ok(questions);
        }


    }
}
