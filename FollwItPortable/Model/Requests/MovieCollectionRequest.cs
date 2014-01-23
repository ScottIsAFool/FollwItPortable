using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class MovieCollectionRequest : MovieBaseRequest
    {
        [JsonProperty("insert_in_stream")]
        public bool InsertInStream { get; set; }
    }
}