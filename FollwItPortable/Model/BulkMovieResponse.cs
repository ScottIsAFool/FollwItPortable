using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class BulkMovieResponse
    {
        [JsonProperty("client_id")]
        public int ClientId { get; set; }

        [JsonProperty("movie_id")]
        public string MovieId { get; set; }

        [JsonProperty("in_collection")]
        public bool InCollection { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("watched")]
        public bool Watched { get; set; }
    }
}
