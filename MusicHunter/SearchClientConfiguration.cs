namespace Uthef.MusicHunter
{
    public class SearchClientConfiguration
    {
        public string? SpotifyClientId { get; }
        public string? SpotifyClientSecret { get; }

        public string? SoundCloudClientId;
        public bool UseSoundCloudProxy;

        public SearchClientConfiguration(
            string? spotifyClientId = null, 
            string? spotifyClientSecret = null, 
            string? soundCloudClientId = "0K8gqs6E9DAVUafZxVq6xIIVVjtIgXTv",
            bool useSoundCloudProxy = false)
        {
            SpotifyClientId = spotifyClientId;
            SpotifyClientSecret = spotifyClientSecret;
            SoundCloudClientId = soundCloudClientId;
            UseSoundCloudProxy = useSoundCloudProxy;
        }
    }
}
