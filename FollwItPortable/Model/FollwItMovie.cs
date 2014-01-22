using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItMovie
    {
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
    }
}
