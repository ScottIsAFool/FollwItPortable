using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItEpisodeSummary
    {
        [JsonProperty("episode_id")]
        public string EpisodeId { get; set; }

        [JsonProperty("tvdb_episode_id")]
        public string TvdbEpisodeId { get; set; }

        [JsonProperty("season_number")]
        public string SeasonNumber { get; set; }

        [JsonProperty("episode_number")]
        public string EpisodeNumber { get; set; }

        [JsonProperty("in_collection")]
        public bool InCollection { get; set; }

        [JsonProperty("watched")]
        public bool Watched { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("quick_thoughts")]
        public string QuickThoughts { get; set; }

        [JsonProperty("review")]
        public string Review { get; set; }

        [JsonProperty("want_it")]
        public bool WantIt { get; set; }

        [JsonProperty("not_interested")]
        public bool NotInterested { get; set; }
    }
}