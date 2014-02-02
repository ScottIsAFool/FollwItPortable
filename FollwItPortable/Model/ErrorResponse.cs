using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class ErrorResponse
    {
        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

}
