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

        [JsonPropertyName("artwork_url")]
        public string? ArtworkUrl { get; }

        [JsonConstructor]
        public SoundCloudItem(long id, SoundCloudUser user, string title, string link, string? artworkUrl)
        {
            Id = id;
            User = user;
            Title = title;
            Link = link;
            ArtworkUrl = artworkUrl;
        }
    }
}
