namespace Uthef.MusicResolver
{
    public class ArtworkResolution
    {
        public string Value { get; }

        private ArtworkResolution(string value) => Value = value;
        
        public readonly static ArtworkResolution Small = new("100x100");
        public readonly static ArtworkResolution Medium = new("600x600");
        public readonly static ArtworkResolution Large = new("1000x1000");

        public static implicit operator string(ArtworkResolution resolution) => resolution.Value; 
    }
}
