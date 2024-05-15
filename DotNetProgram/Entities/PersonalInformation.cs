using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace dotnetProgram.Entities
{
    public class PersonalInformation
    {
        [JsonProperty(PropertyName = "id")]
        public string IDNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
       
        public string Nationality { get; set; }

        public string CurrentResidence { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }
    }
}
