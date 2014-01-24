using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class QueryUsernameRequest : MovieBaseRequest
    {
        [JsonProperty("query_username")]
        public string QueryUsername { get; set; }
    }
}