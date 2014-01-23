using System.Collections.Generic;
using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class BulkMovieChangeRequest : BaseRequest
    {
        [JsonProperty("movies")]
        public List<BulkMovie> Movies { get; set; }

        [JsonProperty("in_collection")]
        public bool? InCollection { get; set; }

        [JsonProperty("watched")]
        public bool? Watched { get; set; }

        [JsonProperty("rating")]
        public int? Rating { get; set; }
    }

    internal class BulkMovie : FollwItMovie
    {
        [JsonProperty("resources")]
        public string Resources { get; set; }
    }
}
