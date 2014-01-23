using System.Collections.Generic;
using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class BulkEpisodeChangeRequest : BaseRequest
    {
        [JsonProperty("episodes")]
        public List<FollwItEpisode> Episodes { get; set; }
        [JsonProperty("in_collection")]
        public bool? InCollection { get; set; }
        [JsonProperty("watched")]
        public bool? Watched { get; set; }
        [JsonProperty("rating")]
        public int? Rating { get; set; }
    }
}