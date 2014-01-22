using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItTvShow
    {
        [JsonProperty("follwit_series_id")]
        public int FollwitSeriesId { get; set; }

        [JsonProperty("thetvdb_series_id")]
        public int ThetvdbSeriesId { get; set; }

        [JsonProperty("series_name")]
        public string SeriesName { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("average_rating")]
        public string AverageRating { get; set; }

        [JsonProperty("rating_count")]
        public string RatingCount { get; set; }

        [JsonProperty("runtime")]
        public string Runtime { get; set; }

        [JsonProperty("first_aired")]
        public string FirstAired { get; set; }

        [JsonProperty("air_day")]
        public string AirDay { get; set; }

        [JsonProperty("air_time")]
        public string AirTime { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("series_url")]
        public string SeriesUrl { get; set; }

        [JsonProperty("series_poster")]
        public string SeriesPoster { get; set; }

        [JsonProperty("series_poster_med")]
        public string SeriesPosterMed { get; set; }

        [JsonProperty("series_poster_small")]
        public string SeriesPosterSmall { get; set; }

        [JsonProperty("actors")]
        public FollwItActor[] Actors { get; set; }

        [JsonProperty("genres")]
        public string Genres { get; set; }

        [JsonProperty("episodes")]
        public FollwItEpisode[] Episodes { get; set; }
    }
}
