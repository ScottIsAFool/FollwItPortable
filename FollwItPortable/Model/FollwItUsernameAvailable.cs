using Newtonsoft.Json;

namespace FollwItPortable.Model
{
    internal class FollwItUsernameAvailable
    {
        [JsonProperty("username")]
        internal string Username { get; set; }

        [JsonProperty("available")]
        internal bool Available { get; set; }
    }
}
