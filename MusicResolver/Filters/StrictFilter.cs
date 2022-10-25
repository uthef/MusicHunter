using System.Text.RegularExpressions;

namespace Uthef.MusicResolver.Filters
{
    public sealed class StrictFilter : IMusicResolverFilter
    {
        public string? Artist { get; }
        public string? Title { get; }
        public int Limit { get; }

        public bool ExtractArtistNameFromTitle = true;

        public StrictFilter(string? artist, string? title, int limit = MusicResolver.DefaultLimit)
        {
            Artist = artist;
            Title = title;
            Limit = limit;
        }

        public bool IsItemValid(SearchItem item, ItemType type)
        {
            if (Artist != null && !item.CompareArtist(Artist))
            {
                if (!ExtractArtistNameFromTitle || (item.Service != MusicService.YouTube && item.Service != MusicService.SoundCloud))
                    return false;
    
                var regex = new Regex($"^{Regex.Escape(Artist)}(?=\\s*(-|—|–))", RegexOptions.IgnoreCase);
                var match = regex.Match(item.Title);

                if (match.Success) 
                    item.Artists.Add(match.Value);
                else 
                    return false;
            }

            if (Title != null && !item.CompareTitle(Title))
            {
                if (item.Service != MusicService.YouTube && item.Service != MusicService.SoundCloud)
                    return false;

                var regex = new Regex($"(?<=(-|—|–)\\s*){Regex.Escape(Title)}", RegexOptions.IgnoreCase);
                var match = regex.Match(item.Title);

                if (match.Success) 
                    item.Title = match.Value;
                else 
                    return false;
            }

            return true;
        }
    }
}
