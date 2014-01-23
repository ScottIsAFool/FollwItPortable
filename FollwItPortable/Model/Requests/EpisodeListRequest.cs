using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class EpisodeListRequest : EpisodeBaseRequest
    {
        [JsonProperty("list_id")]
        public string ListId { get; set; }
    }
}