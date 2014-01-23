using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class ShowUserStatsRequest : ShowBaseRequest
    {
        [JsonProperty("query_username")]
        public string QueryUsername { get; set; }
        [JsonProperty("include_episodes")]
        public bool IncludeEpisodes { get; set; }
    }
}