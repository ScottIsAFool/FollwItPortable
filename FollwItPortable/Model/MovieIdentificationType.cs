using FollwItPortable.Attributes;

namespace FollwItPortable.Model
{
    public enum MovieIdentificationType
    {
        [Description("movie_id")]
        FollwIt,
        [Description("imdb_id")]
        Imdb,
        [Description("tmdb_id")]
        Tmdb
    }
}