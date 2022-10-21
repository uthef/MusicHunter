using System.Text.Json.Serialization;

namespace Uthef.MusicResolver.BandcampModels
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
