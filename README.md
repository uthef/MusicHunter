# MusicResolver

### Supported services
- Yandex Music API
- Apple Music API
- Deezer API
- YouTube Music (via public YouTube API)
- Spotify API (requires client ID and client secret)
- SoundCloud (partial support, web API)
- Bandcamp (partial support, web API)
- Amazon Music (partial support via parsing HTML page)

### Known limitations
- Amazon can return up to 16 results (1 page)
- Bandcamp can return up to 50 results
- ```MusicResolver.DefaultArtworkResolution``` property only applies to Yandex Music, Deezer & Apple Music
- ```MusicResolverConfiguration.UseSoundCloudProxy``` property is used for testing purposes and not meant for production

### Examples
- See MusicResolverTests project
