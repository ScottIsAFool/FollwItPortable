using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class MovieListRequest : MovieBaseRequest
    {
        [JsonProperty("list_id")]
        public string ListId { get; set; }
    }
}