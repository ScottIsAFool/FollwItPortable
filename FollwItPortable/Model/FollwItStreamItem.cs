using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItStreamItem
    {
        [JsonProperty("stream_id")]
        public int StreamId { get; set; }

        [JsonProperty("action")]
        public StreamAction Action { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("item_type")]
        public string ItemType { get; set; }

        [JsonProperty("follwit_movie_id")]
        public int FollwitMovieId { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("moviedb_id")]
        public string MoviedbId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("certification")]
        public string Certification { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("quick_thoughts")]
        public string QuickThoughts { get; set; }

        [JsonProperty("rating")]
        public int? Rating { get; set; }

        [JsonProperty("follwit_series_id")]
        public int? FollwitSeriesId { get; set; }

        [JsonProperty("follwit_episode_id")]
        public int? FollwitEpisodeId { get; set; }

        [JsonProperty("thetvdb_series_id")]
        public string ThetvdbSeriesId { get; set; }

        [JsonProperty("thetvdb_episode_id")]
        public string ThetvdbEpisodeId { get; set; }

        [JsonProperty("series_name")]
        public string SeriesName { get; set; }

        [JsonProperty("series_url")]
        public string SeriesUrl { get; set; }

        [JsonProperty("series_poster")]
        public string SeriesPoster { get; set; }

        [JsonProperty("season_number")]
        public int? SeasonNumber { get; set; }

        [JsonProperty("episode_number")]
        public int? EpisodeNumber { get; set; }

        [JsonProperty("episode_name")]
        public string EpisodeName { get; set; }

        [JsonProperty("episode_url")]
        public string EpisodeUrl { get; set; }

        [JsonProperty("episode_image")]
        public string EpisodeImage { get; set; }

        [JsonProperty("first_aired")]
        public string FirstAired { get; set; }
    }
}
