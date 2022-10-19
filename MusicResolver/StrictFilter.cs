namespace Uthef.MusicResolver
{
    public sealed class StrictFilter : IMusicResolverFilter
    {
        public readonly string? Artist;
        public readonly string? Title;

        public int Limit { get; }

        public StrictFilter(string? artist, string? title, int limit = MusicResolver.DefaultLimit)
        {
            Artist = artist;
            Title = title;
            Limit = limit;
        }

        public bool IsItemValid(SearchItem item, ItemType type)
        {
            if (Artist != null && !item.CompareArtist(Artist))
                return false;

            if (Title != null && !item.CompareTitle(Title))
            {
                if (Artist == null || item.Service != MusicService.YouTube) 
                    return false;

                if (!item.TitleStartsWith($"{Artist.Trim()} - {Title.Trim()}"))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
