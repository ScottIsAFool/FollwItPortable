using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class EpisodeResponse
    {
        [JsonProperty("client_id")]
        public int ClientId { get; set; }

        [JsonProperty("episode_id")]
        public int EpisodeId { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("watched")]
        public bool Watched { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

}
