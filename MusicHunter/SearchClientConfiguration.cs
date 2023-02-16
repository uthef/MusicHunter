using Newtonsoft.Json;

namespace Uthef.MusicHunter
{
    public class SearchClientConfiguration
    {
        public string? SpotifyClientId { get; }
        public string? SpotifyClientSecret { get; }

        public string? SoundCloudClientId { get; }
        public bool UseSoundCloudProxy { get; }

        [JsonConstructor]
        public SearchClientConfiguration(
            string? spotifyClientId = null, 
            string? spotifyClientSecret = null, 
            string? soundCloudClientId = null,
            bool useSoundCloudProxy = false)
        {
            SpotifyClientId = spotifyClientId;
            SpotifyClientSecret = spotifyClientSecret;
            SoundCloudClientId = soundCloudClientId;
            UseSoundCloudProxy = useSoundCloudProxy;
        }
    }
}
