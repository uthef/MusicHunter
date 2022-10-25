namespace Uthef.MusicResolver
{
    public class SearchItem
    {   
        public string Id { get; }
        public string Url { get; }
        public string Title { get; internal set; }
        public List<string> Artists { get; }
        public string? ArtworkUrl { get; }
        public bool HasArtwork { get => ArtworkUrl != null; }
        public MusicService Service { get; }
        public TimeSpan ExecutionTime { get; internal set; }

        public SearchItem(string id, string url, string title, string artist, MusicService service)
        {
            Id = id;
            Url = url;
            Title = title;
            Artists = new()
            {
                artist
            };
            Service = service;
        }

        public SearchItem(string id, string url, string title, string artist, string? artworkUrl, MusicService service)
        {
            Id = id;
            Url = url;
            Title = title;
            Artists = new()
            {
                artist
            };
            Service = service;
            ArtworkUrl = artworkUrl;
        }

        public SearchItem(string id, string url, string title, IEnumerable<string> artists, MusicService service)
        {
            Id = id;
            Url = url;
            Title = title;
            Artists = artists.ToList();
            Service = service;
        }

        public SearchItem(string id, string url, string title, IEnumerable<string> artists, string? artworkUrl, MusicService service)
        {
            Id = id;
            Url = url;
            Title = title;
            Artists = artists.ToList();
            ArtworkUrl = artworkUrl;
            Service = service;
        }

        public bool CompareArtist(string artist) =>
            Artists.Find(x => x.ToLower().Trim() == artist.ToLower().Trim()) != null;

        public bool CompareTitle(string title) =>
            Title.ToLower().Trim() == title?.ToLower().Trim();
    }
}
