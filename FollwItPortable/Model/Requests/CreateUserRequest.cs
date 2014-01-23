using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class CreateUserRequest : BaseRequest
    {
        [JsonProperty("email")]
        public string EmailAddress { get; set; }
        [JsonProperty("locale")]
        public string Locale { get; set; }
        [JsonProperty("private_profile")]
        public bool PrivateProfile { get; set; }
    }
}