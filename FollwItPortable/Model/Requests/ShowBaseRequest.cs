using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class ShowBaseRequest : BaseRequest
    {
        [JsonProperty("show_id")]
        public int? ShowId { get; set; }
        [JsonProperty("tvdb_series_id")]
        public int? TvdbId { get; set; }
    }
}