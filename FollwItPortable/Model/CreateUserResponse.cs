using Newtonsoft.Json;

namespace FollwItPortable.Model
{
    internal class CreateUserResponse
    {
        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
