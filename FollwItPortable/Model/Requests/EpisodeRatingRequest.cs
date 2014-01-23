using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class EpisodeRatingRequest : EpisodeBaseRequest
    {
        [JsonProperty("rating")]
        public int Rating { get; set; }
    }
}