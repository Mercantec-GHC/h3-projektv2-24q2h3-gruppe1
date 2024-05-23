using System.Text.Json.Serialization;

namespace API.Models
{
    public class User : Common
    {
        [JsonPropertyName("email")]

        public string Email { get; set; }
        [JsonPropertyName("username")]

        public string Username { get; set; }
        [JsonPropertyName("password")]

        public string Password { get; set; }
        [JsonPropertyName("salt")]

        public string Salt { get; set; }
    }

    public class UserSignUpRequest
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class UserPutRequest
    {
        public string Username { get; set; }
       public string Password { get; set; }
    }
}
