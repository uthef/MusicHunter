using System.Text.Json.Serialization;

namespace Uthef.MusicResolver.SoundCloudModels
{
    internal class SoundCloudItem
    {
        public long Id { get; }
        public SoundCloudUser User { get; }
        public string Title { get; }

        [JsonPropertyName("permalink_url")]
        public string Link { get; }

        [JsonConstructor]
        public SoundCloudItem(long id, SoundCloudUser user, string title, string link)
        {
            Id = id;
            User = user;
            Title = title;
            Link = link;
        }
    }
}
