using FollwItPortable.Attributes;

namespace FollwItPortable.Model
{
    public enum ShowIdentificationType
    {
        [Description("movie_id")]
        FollwIt,
        [Description("imdb_id")]
        Imdb,
        [Description("tvdb_id")]
        Tvdb
    }
}