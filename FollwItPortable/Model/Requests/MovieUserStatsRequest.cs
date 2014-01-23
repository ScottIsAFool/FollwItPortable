using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class MovieUserStatsRequest : MovieBaseRequest
    {
        [JsonProperty("query_username")]
        public string QueryUsername { get; set; }
    }
}