using Newtonsoft.Json;

namespace dotnetProgram.Entities
{
    public class Questions
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }  

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Options { get; set; }

        [JsonProperty("maxLength", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxLength { get; set; }  //in case of paragraphs lets set a max length

        [JsonProperty("range", NullValueHandling = NullValueHandling.Ignore)]
        public Range? Range { get; set; }// for numeric questions

    }
}
