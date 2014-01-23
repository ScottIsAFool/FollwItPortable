using FollwItPortable.Attributes;

namespace FollwItPortable.Model
{
    public enum ChangeType
    {
        [Description("server_time")]
        ServerTime,
        [Description("new_movie_rating")]
        NewMovieRating,
        [Description("new_movie_watched_status")]
        NewMovieWatchedStatus,
        [Description("cover_request")]
        CoverRequest,
        [Description("updated_movie_id")]
        UpdatedMovieId,
        [Description("new_series_rating")]
        NewSeriesRating,
        [Description("new_episode_rating")]
        NewEpisodeRating,
        [Description("new_episode_watched_status")]
        NewEpisodeWatchedStatus
    }
}