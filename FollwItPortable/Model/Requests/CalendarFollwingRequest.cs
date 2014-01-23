using System;
using Newtonsoft.Json;

namespace FollwItPortable.Model.Requests
{
    internal class CalendarFollwingRequest : BaseRequest
    {
        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }
        [JsonProperty("end_date")]
        public DateTime EndDate { get; set; }
        [JsonProperty("locale")]
        public string Locale { get; set; }
    }
}
