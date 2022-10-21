using System.Text.Json.Serialization;

namespace Uthef.MusicResolver.BandcampModels
{
    public class BandcampSearchResult
    {
        [JsonPropertyName("auto")]
        public BandcampResultContainer Data { get; }

        public BandcampSearchResult(BandcampResultContainer data)
        {
            Data = data;
        }
    }
}
