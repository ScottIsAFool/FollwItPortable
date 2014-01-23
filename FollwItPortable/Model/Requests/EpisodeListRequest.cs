using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class EpisodeListRequest : EpisodeCollectionRequest
    {
        [JsonProperty("list_id")]
        public string ListId { get; set; }
    }
}