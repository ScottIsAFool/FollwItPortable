using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class ShowRatingRequest : ShowBaseRequest
    {
        [JsonProperty("rating")]
        public int Rating { get; set; }
    }
}