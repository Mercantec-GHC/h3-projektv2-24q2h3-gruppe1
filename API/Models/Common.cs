using System.Text.Json.Serialization;

namespace API.Models
{
    public class Common
    {
        [JsonPropertyName("id")]

        public int Id { get; set; }
        [JsonPropertyName("createdAt")]

        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")]

        public DateTime UpdatedAt { get; set; }
    }
}
