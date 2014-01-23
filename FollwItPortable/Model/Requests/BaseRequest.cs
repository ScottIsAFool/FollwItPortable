using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal abstract class BaseRequest
    {
        [JsonProperty("username")]
        internal string Username { get; set; }
        [JsonProperty("password")]
        internal string Password { get; set; }
    }
}
