using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItMovie
    {
        public string Id
        {
            get { return string.IsNullOrEmpty(MovieId) ? FollwitMovieId.ToString() : MovieId; }
        }

        [JsonProperty("movie_id")]
        public string MovieId { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [JsonProperty("translated_title")]
        public string TranslatedTitle { get; set; }

        [JsonProperty("tagline")]
        public string Tagline { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("cover_image")]
        public string CoverImage { get; set; }

        [JsonProperty("certification")]
        public string Certification { get; set; }

        [JsonProperty("spoken_language")]
        public string SpokenLanguage { get; set; }

        [JsonProperty("runtime")]
        public string Runtime { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("average_rating")]
        public string AverageRating { get; set; }

        [JsonProperty("rating_count")]
        public string RatingCount { get; set; }

        [JsonProperty("movie_rank")]
        public string MovieRank { get; set; }

        [JsonProperty("genres")]
        public string Genres { get; set; }

        [JsonProperty("trailers")]
        public FollwItTrailer[] Trailers { get; set; }

        [JsonProperty("directors")]
        public FollwItDirector[] Directors { get; set; }

        [JsonProperty("actors")]
        public FollwItActor[] Actors { get; set; }

        [JsonProperty("writers")]
        public FollwItWriter[] Writers { get; set; }

        [JsonProperty("follwit_movie_id")]
        public int FollwitMovieId { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("moviedb_id")]
        public string TmdbId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("cover_med")]
        public string CoverMed { get; set; }

        [JsonProperty("cover_small")]
        public string CoverSmall { get; set; }

        [JsonProperty("user_count")]
        public int UserCount { get; set; }

        [JsonProperty("cast")]
        public string Cast { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("localized_title")]
        public string LocalizedTitle { get; set; }

        [JsonProperty("localized_tagline")]
        public string LocalizedTagline { get; set; }

        [JsonProperty("localized_summary")]
        public string LocalizedSummary { get; set; }

        [JsonProperty("in_collection")]
        public bool InCollection { get; set; }

        [JsonProperty("watched")]
        public bool Watched { get; set; }

        [JsonProperty("last_watch_date")]
        public string LastWatchDate { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("file_hash")]
        public string FileHash { get; set; }

        [JsonProperty("client_id")]
        public int ClientId { get; set; }
    }
}
