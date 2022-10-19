using System.Text.Json.Serialization;

namespace Uthef.MusicReolver.SoundCloudModels
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
