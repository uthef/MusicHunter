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
            string? soundCloudClientId = "jOJjarVXJfZlI309Up55k93EUDG7ILW6",
            bool useSoundCloudProxy = false)
        {
            SpotifyClientId = spotifyClientId;
            SpotifyClientSecret = spotifyClientSecret;
            SoundCloudClientId = soundCloudClientId;
            UseSoundCloudProxy = useSoundCloudProxy;
        }
    }
}
