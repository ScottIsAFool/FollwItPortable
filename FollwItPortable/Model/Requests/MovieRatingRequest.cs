using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class MovieRatingRequest : MovieBaseRequest
    {
        [JsonProperty("rating")]
        public int Rating { get; set; }
    }
}