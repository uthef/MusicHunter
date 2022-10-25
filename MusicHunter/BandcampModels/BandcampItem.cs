using System.Text.Json.Serialization;

namespace Uthef.MusicHunter.BandcampModels
{
    public class BandcampItem
    {
        public long Id { get; }
        public string Name { get; }

        [JsonPropertyName("band_name")]
        public string Band { get; }

        [JsonPropertyName("item_url_path")]
        public string Url { get; }

        [JsonPropertyName("img")]
        public string? Image { get; }

        public BandcampItem(long id, string name, string band, string url, string? image)
        {
            Id = id;
            Name = name;
            Band = band;
            Url = url;
            Image = image?.Replace("img/", "img/a");
        }
    }
}
