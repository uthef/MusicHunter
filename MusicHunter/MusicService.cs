using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Uthef.MusicHunter
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MusicService
    {
        YouTube = 1, 
        Bandcamp = 2, 
        Deezer = 4,
        AppleMusic = 8,
        Spotify = 16, 
        Yandex = 32, 
        SoundCloud = 64, 
        Amazon = 128
    }
}
