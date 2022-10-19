using Uthef.MusicResolver;

namespace AspNetCoreWebApiExample
{
    public class ResolverRequestBody
    {
        public string Query { get; set; } = "";
        public HashSet<MusicService> Services { get; set; } = new HashSet<MusicService>();
        public string? StrictArtist { get; set; }
        public string? StrictTitle { get; set; }
        public ItemType Type { get; set; } = ItemType.Track;
    }
}
