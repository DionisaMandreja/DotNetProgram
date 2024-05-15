using Newtonsoft.Json;

namespace dotnetProgram.Entities
{
    public class Applications
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("applicantInfo")]
        public PersonalInformation PersonalInformation { get; set; } 

        [JsonProperty("responses")]
        public List<Response> Responses { get; set; }

        [JsonProperty("programId")]

        public string ProgramId { get; set; }
    }
}
