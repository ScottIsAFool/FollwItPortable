using Newtonsoft.Json;

namespace FollwItPortable.Model.Responses
{
    internal class AuthenticationResponse
    {
        [JsonProperty("response")]
        internal string Response { get; set; }

        [JsonProperty("username")]
        internal string Username { get; set; }
    }

}
