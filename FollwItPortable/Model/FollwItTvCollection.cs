using Newtonsoft.Json;
using PropertyChanged;

namespace FollwItPortable.Model
{
    [ImplementPropertyChanged]
    public class FollwItTvCollection
    {
        [JsonProperty("follwit_series_id")]
        public int FollwitSeriesId { get; set; }

        [JsonProperty("thetvdb_series_id")]
        public int? ThetvdbSeriesId { get; set; }

        [JsonProperty("series_name")]
        public string SeriesName { get; set; }

        [JsonProperty("series_url")]
        public string SeriesUrl { get; set; }

        [JsonProperty("series_poster")]
        public string SeriesPoster { get; set; }

        [JsonProperty("episodes")]
        public FollwItEpisode[] Episodes { get; set; }
    }

}
