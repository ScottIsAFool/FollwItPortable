using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItUser
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("real_name")]
        public string RealName { get; set; }

        [JsonProperty("last_seen")]
        public string LastSeen { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("friends")]
        public FollwItFriend[] Friends { get; set; }

        [JsonProperty("achievements")]
        public FollwItAchievement[] Achievements { get; set; }

        [JsonProperty("watching_movie")]
        public FollwItWatchingMovie[] WatchingMovie { get; set; }

        [JsonProperty("watched_movie")]
        public FollwItWatchedMovie WatchedMovie { get; set; }

        [JsonProperty("watching_episode")]
        public FollwItWatchingEpisode[] WatchingEpisode { get; set; }

        [JsonProperty("watched_episode")]
        public FollwItWatchedEpisode WatchedEpisode { get; set; }
    }
}
