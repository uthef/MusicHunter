using System.Text.Json.Serialization;
using Uthef.MusicHunter;

namespace Uthef.MusicHunter.BandcampModels
{
    public class BandcampSearchRequest
    {
        [JsonPropertyName("search_text")]
        public string SearchText { get; }

        [JsonIgnore]
        public ItemType Filter { get; }

        [JsonPropertyName("search_filter")]
        public string FilterAsString
        {
            get => Filter is ItemType.Track ? "t" : "a";
        }

        [JsonPropertyName("fan_id")]
        public long FanId { get; }

        [JsonPropertyName("full_page")]
        public bool FullPage { get; }

        public BandcampSearchRequest(string searchText, ItemType filter, long fanId, bool fullPage)
        {
            SearchText = searchText;
            Filter = filter;
            FanId = fanId;
            FullPage = fullPage;
        }
    }
}
