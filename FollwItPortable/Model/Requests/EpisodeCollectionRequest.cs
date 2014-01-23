using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class EpisodeCollectionRequest : BaseRequest
    {
        [JsonProperty("episode_id")]
        public int? EpisodeId { get; set; }

        [JsonProperty("tvdb_episode_id")]
        public int? TvdbEpisodeId { get; set; }

        [JsonProperty("series_name")]
        public string SeriesName { get; set; }

        [JsonProperty("season_number")]
        public int? SeasonNumber { get; set; }

        [JsonProperty("episode_number")]
        public int? EpisodeNumber { get; set; }

        [JsonProperty("episode_name")]
        public string EpisodeName { get; set; }
    }
}
