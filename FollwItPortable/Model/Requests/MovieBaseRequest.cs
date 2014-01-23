using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class MovieBaseRequest : BaseRequest
    {
        [JsonProperty("movie_id")]
        public int? MovieId { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("tmdb_id")]
        public int? TmdbId { get; set; }
    }
}
