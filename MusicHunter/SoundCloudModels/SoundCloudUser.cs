using System.Text.Json.Serialization;

namespace Uthef.MusicHunter.SoundCloudModels
{

    internal class SoundCloudUser
    {
        [JsonPropertyName("username")]
        public string Name { get; }

        [JsonConstructor]
        public SoundCloudUser(string name)
        {
            Name = name;
        }
    }
}
