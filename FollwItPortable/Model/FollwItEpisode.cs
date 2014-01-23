using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItEpisode
    {
        [JsonProperty("follwit_series_id")]
        public int FollwitSeriesId { get; set; }

        [JsonProperty("thetvdb_series_id")]
        public int ThetvdbSeriesId { get; set; }

        [JsonProperty("series_name")]
        public string SeriesName { get; set; }

        [JsonProperty("series_url")]
        public string SeriesUrl { get; set; }

        [JsonProperty("series_poster")]
        public string SeriesPoster { get; set; }

        [JsonProperty("follwit_episode_id")]
        public int FollwitEpisodeId { get; set; }

        [JsonProperty("thetvdb_episode_id")]
        public int ThetvdbEpisodeId { get; set; }

        [JsonProperty("season_number")]
        public int SeasonNumber { get; set; }

        [JsonProperty("episode_number")]
        public int EpisodeNumber { get; set; }

        [JsonProperty("episode_name")]
        public string EpisodeName { get; set; }

        [JsonProperty("episode_url")]
        public string EpisodeUrl { get; set; }

        [JsonProperty("episode_image")]
        public string EpisodeImage { get; set; }

        [JsonProperty("air_time")]
        public string AirTime { get; set; }

        [JsonProperty("series")]
        public FollwItTvShow Series { get; set; }
    }
}
