using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class UserListRequest : BaseRequest
    {
        [JsonProperty("query_username")]
        public string QueryUsername { get; set; }
        [JsonProperty("list_id")]
        public string ListId { get; set; }
    }
}