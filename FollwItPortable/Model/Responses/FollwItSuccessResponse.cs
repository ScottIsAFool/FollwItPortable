using Newtonsoft.Json;

namespace FollwItPortable.Model.Responses
{

    internal class FollwItSuccessResponse
    {
        [JsonProperty("response")]
        public string Response { get; set; }
    }

}
