using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class EpisodeWatchedRequest : EpisodeBaseRequest
    {
        [JsonProperty("insert_in_stream")]
        public bool InsertInStream { get; set; }
    }
}