using FollwItPortable.Attributes;
using FollwItPortable.Converters;
using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItListEntry
    {
        [JsonProperty("date_added")]
        public string DateAdded { get; set; }

        [JsonProperty("item_type")]
        [JsonConverter(typeof(ListTypeConverter))]
        public ListType ItemType { get; set; }

        [JsonProperty("follwit_movie_id")]
        public int? FollwitMovieId { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("moviedb_id")]
        public string MoviedbId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("follwit_series_id")]
        public int? FollwitSeriesId { get; set; }

        [JsonProperty("thetvdb_series_id")]
        public int? ThetvdbSeriesId { get; set; }

        [JsonProperty("series_name")]
        public string SeriesName { get; set; }

        [JsonProperty("series_url")]
        public string SeriesUrl { get; set; }

        [JsonProperty("series_poster")]
        public string SeriesPoster { get; set; }

        [JsonProperty("season_number")]
        public int? SeasonNumber { get; set; }

        [JsonProperty("follwit_episode_id")]
        public int? FollwitEpisodeId { get; set; }

        [JsonProperty("thetvdb_episode_id")]
        public int? ThetvdbEpisodeId { get; set; }

        [JsonProperty("episode_number")]
        public int? EpisodeNumber { get; set; }

        [JsonProperty("episode_name")]
        public string EpisodeName { get; set; }

        [JsonProperty("episode_url")]
        public string EpisodeUrl { get; set; }

        [JsonProperty("episode_image")]
        public string EpisodeImage { get; set; }
    }

    public enum ListType
    {
        [Description("tv show")]
        TvShow,
        [Description("tv season")]
        TvSeason,
        [Description("tv episode")]
        TvEpisode,
        [Description("movie")]
        Movie
    }
}