using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItTvUserStats
    {
        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("quick_thoughts")]
        public string QuickThoughts { get; set; }

        [JsonProperty("review")]
        public string Review { get; set; }

        [JsonProperty("want_it")]
        public bool WantIt { get; set; }

        [JsonProperty("not_interested")]
        public bool NotInterested { get; set; }

        [JsonProperty("follwing")]
        public bool Follwing { get; set; }

        [JsonProperty("episodes")]
        public FollwItEpisodeSummary[] Episodes { get; set; }
    }
}
