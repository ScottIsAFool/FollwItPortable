using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class RecommendedRequest : BaseRequest
    {
        [JsonProperty("genres")]
        public string Genres { get; set; }
    }
}