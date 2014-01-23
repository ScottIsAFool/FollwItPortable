using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class ShowListRequest : ShowBaseRequest
    {
        [JsonProperty("list_id")]
        public string ListId { get; set; }
    }
}