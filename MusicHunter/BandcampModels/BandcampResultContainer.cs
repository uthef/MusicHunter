using System.Text.Json.Serialization;

namespace Uthef.MusicHunter.BandcampModels
{
    public class BandcampResultContainer
    {
        [JsonPropertyName("results")]
        public List<BandcampItem> Items { get; }

        public BandcampResultContainer(List<BandcampItem> items)
        {
            Items = items;
        }
    }
}
