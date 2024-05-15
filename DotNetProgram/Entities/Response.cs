using Newtonsoft.Json;

namespace dotnetProgram.Entities
{
    public class Response
    {
        [JsonProperty("questionId")]
        public string QuestionId { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
