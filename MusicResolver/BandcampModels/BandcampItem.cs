using System.Text.Json.Serialization;

namespace Uthef.MusicResolver.BandcampModels
{
    public class BandcampItem
    {
        public long Id { get; }
        public string Name { get; }

        [JsonPropertyName("band_name")]
        public string Band { get; }

        [JsonPropertyName("item_url_path")]
        public string Url { get; }

        public BandcampItem(long id, string name, string band, string url)
        {
            Id = id;
            Name = name;
            Band = band;
            Url = url;
        }
    }
}
