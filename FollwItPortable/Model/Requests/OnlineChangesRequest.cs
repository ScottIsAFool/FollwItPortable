using System;
using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class OnlineChangesRequest : BaseRequest
    {
        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }
    }
}