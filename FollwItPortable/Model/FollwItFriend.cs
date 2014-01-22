using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItFriend
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("real_name")]
        public string RealName { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }
}