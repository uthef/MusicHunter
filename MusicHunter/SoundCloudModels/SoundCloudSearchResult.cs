using System.Text.Json.Serialization;

namespace Uthef.MusicHunter.SoundCloudModels
{
    internal class SoundCloudSearchResult
    {
        public List<SoundCloudItem> Collection { get; }

        [JsonConstructor]
        public SoundCloudSearchResult(List<SoundCloudItem> collection)
        {
            Collection = collection;
        }
    }
}
