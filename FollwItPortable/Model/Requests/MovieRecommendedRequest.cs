using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class MovieRecommendedRequest : BaseRequest
    {
        [JsonProperty("genres")]
        public string Genres { get; set; }
    }
}